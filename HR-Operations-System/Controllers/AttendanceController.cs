using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static HR_Operations_System.Controllers.AttendancesController;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<AttendancesController> _logger;

        public AttendancesController(IRepository rep, ILogger<AttendancesController> logger)
        {
            _rep = rep;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAttendance")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendance()
        {
            var result = await _rep.GetAsync<Attendance>();
            return Ok(result);
        }


        [HttpPost]
        [Route("AddAttendance")]
        public async Task<ActionResult> AddAttendance(Attendance entity)
        {
            await _rep.AddAsync(entity);
            return Ok();
        }

        public class GetAttendanceDay
        {
            public DateOnly Day { get; set; }
            public int FingerCode { get; set; }
        }

        [HttpPost]
        [Route("GetAttendanceOfDay")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendanceOfDay(GetAttendanceDay getAttendanceDay)
        {
            DateTime dateTime = getAttendanceDay.Day.ToDateTime(new TimeOnly());
            var result = await _rep.GetListByAsync<Attendance>(x => x.FingerCode == getAttendanceDay.FingerCode && x.IODateTime.Date == dateTime);
            return Ok(result);
        }


        public class EmployeCalculationRequestBody
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int FingerCode { get; set; }
        }


        [HttpPost]
        [Route("EmployeeCalculation")]
        public async Task<ActionResult<IEnumerable<Transaction>>> EmployeeCalculation(EmployeCalculationRequestBody employeeCalculation)
        {
            DateTime startDate = new DateTime(employeeCalculation.Year, employeeCalculation.Month, 1);

            List<Transaction> transactionList = new List<Transaction>();
            for (DateTime date = startDate; date.Month == employeeCalculation.Month; date = date.AddDays(1))
            {
                if (IsHoliday(date))
                    continue;

                var transactions = await GetTransactionsOfDay(employeeCalculation.FingerCode, date);
                transactionList.AddRange(transactions);
            }
            return Ok(transactionList);
        }

        private static bool IsHoliday(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday;
        }
        [HttpPost]
        [Route("GetTransactionsOfDay")]
        public async Task<IEnumerable<Transaction>> GetTransactionsOfDay(int FingerCode, DateTime dateTime)
        {
            var fingerPrints = await _rep.GetListByAsync<Attendance>(x => x.FingerCode == FingerCode && x.IODateTime.Date == dateTime);
            var employee = await _rep.GetByAsync<Employee>(x => x.Id == FingerCode);
            var timingPlan = await _rep.GetByAsync<TimingPlan>(x => x.Id == employee.TimingCode);
            List<Transaction> transactionList = new List<Transaction>();
            bool wentToWork = false;

            var fingerPrintsBeforeWork = fingerPrints.Where(x => x.IODateTime.TimeOfDay <= timingPlan.FromTime).OrderBy(x => x.IODateTime).ToList();
            var fingerPrintsDuringWork = fingerPrints.Where(x => x.IODateTime.TimeOfDay > timingPlan.FromTime && x.IODateTime.TimeOfDay < timingPlan.ToTime).OrderBy(x => x.IODateTime).ToList();
            var fingerPrintsAfterWork = fingerPrints.Where(x => x.IODateTime.TimeOfDay >= timingPlan.ToTime).OrderBy(x => x.IODateTime).ToList();

            if (!fingerPrints.Any() ||
                !fingerPrintsDuringWork.Any()
                && fingerPrintsBeforeWork.Any() && fingerPrintsBeforeWork.Last().TrType != 0
                && fingerPrintsAfterWork.Any() && fingerPrintsAfterWork.First().TrType != 1)
            {
                transactionList.Add(new Transaction()
                {
                    transactionType = TransactionType.Absent,
                    Date = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day),
                });
                return transactionList;
            }

            TimeSpan? notAtWorkSinceTime;

            if (fingerPrintsBeforeWork.Any() && fingerPrintsBeforeWork.Last().TrType == 0)
            {
                wentToWork = true;
                notAtWorkSinceTime = null;
            }
            else
                notAtWorkSinceTime = timingPlan.FromTime;

            foreach (var attendance in fingerPrintsDuringWork)
            {
                if (notAtWorkSinceTime.HasValue && attendance.TrType == 0)
                {
                    if (attendance.IODateTime.TimeOfDay > timingPlan.ToTime)
                        continue;

                    transactionList.Add(new Transaction() //late or excuse
                    {
                        transactionType = wentToWork ? TransactionType.Excuse : TransactionType.Late,
                        Date = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day),
                        FromTime = notAtWorkSinceTime.Value,
                        ToTime = attendance.IODateTime.TimeOfDay,
                        minutes = (int)attendance.IODateTime.TimeOfDay.TotalMinutes - (int)notAtWorkSinceTime.Value.TotalMinutes,
                    });
                    notAtWorkSinceTime = null;
                    wentToWork = true;
                }
                else if (!notAtWorkSinceTime.HasValue && attendance.TrType == 1)
                    notAtWorkSinceTime = attendance.IODateTime.TimeOfDay;
            }

            if (!wentToWork && fingerPrintsAfterWork.Any() && fingerPrintsAfterWork.First().TrType == 1)
            {
                transactionList.Add(new Transaction() //forgot fingerprint in
                {
                    transactionType = TransactionType.ForgotFingerPrintIn,
                    Date = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day),
                    FromTime = timingPlan.ToTime,
                    ToTime = timingPlan.ToTime,
                    minutes = 5,
                });
            }
            if (wentToWork)
                if (notAtWorkSinceTime.HasValue)
                {
                    transactionList.Add(new Transaction() //excuse when leaving
                    {
                        transactionType = TransactionType.Excuse,
                        Date = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day),
                        FromTime = notAtWorkSinceTime.Value,
                        ToTime = timingPlan.ToTime,
                        minutes = (int)timingPlan.ToTime.TotalMinutes - (int)notAtWorkSinceTime.Value.TotalMinutes,
                    });
                }
                else if (!fingerPrintsAfterWork.Any(x => x.TrType == 1))
                    transactionList.Add(new Transaction() //forgot fingerprint out
                    {
                        transactionType = TransactionType.ForgotFingerPrintOut,
                        Date = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day),
                        FromTime = timingPlan.ToTime,
                        ToTime = timingPlan.ToTime,
                        minutes = 5,
                    });


            return transactionList;
        }

        public enum TransactionType
        {
            Late,
            Excuse,
            ForgotFingerPrintOut,
            ForgotFingerPrintIn,
            Absent,
        }
        public class Transaction
        {
            public TransactionType transactionType { get; set; }
            public DateOnly Date { get; set; }
            public TimeSpan FromTime { get; set; }
            public TimeSpan ToTime { get; set; }
            public int minutes { get; set; }
        }


        [HttpPost]
        [Route("AddDummyData/{FingerCode}")]
        public async Task<ActionResult> AddDummyData(int FingerCode)
        {
            System.Random r = new System.Random();
            List<Node> nodeList = (await _rep.GetAsync<Node>()).ToList();

            DateTime startDate = new DateTime(2026, 4, 1);
            DateTime endDate = new DateTime(2026, 6, 30);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                    continue;

                if (r.NextDouble() < 0.05)
                    continue;

                if (r.NextDouble() < 0.95)
                {
                    Attendance enter = new Attendance()
                    {
                        FingerCode = FingerCode,
                        IODateTime = date.AddMinutes(r.Next(360, 540)),
                        NodeSerialNo = nodeList[r.Next(nodeList.Count)].SerialNo,
                        IsActive = true,
                        TrType = 0,
                    };
                    await _rep.AddAsync(enter);
                }

                if (r.NextDouble() < 0.1)
                {
                    Attendance excuseLeave = new Attendance()
                    {
                        FingerCode = FingerCode,
                        IODateTime = date.AddMinutes(r.Next(570, 670)),
                        NodeSerialNo = nodeList[r.Next(nodeList.Count)].SerialNo,
                        IsActive = true,
                        TrType = 1,
                    };

                    Attendance excuseEnter = new Attendance()
                    {
                        FingerCode = FingerCode,
                        IODateTime = date.AddMinutes(r.Next(680, 780)),
                        NodeSerialNo = nodeList[r.Next(nodeList.Count)].SerialNo,
                        IsActive = true,
                        TrType = 0,
                    };

                    await _rep.AddAsync(excuseLeave);
                    await _rep.AddAsync(excuseEnter);
                }


                if (r.NextDouble() < 0.95)
                {
                    Attendance leave = new Attendance()
                    {
                        FingerCode = FingerCode,
                        IODateTime = date.AddMinutes(r.Next(810, 870)),
                        NodeSerialNo = nodeList[r.Next(nodeList.Count)].SerialNo,
                        IsActive = true,
                        TrType = 1,
                    };
                    await _rep.AddAsync(leave);
                }
            }
            return Ok();
        }


    }


}
