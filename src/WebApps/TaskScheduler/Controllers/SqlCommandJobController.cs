using System;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Entities;
using TaskScheduler.Infrastructure;
using TaskScheduler.Models.SqlCommandJob;

namespace TaskScheduler.Controllers
{
    [GenerateClientActions]
    public class SqlCommandJobController : Controller
    {
        private readonly TaskSchedulerDbContext _db;

        public SqlCommandJobController(TaskSchedulerDbContext db)
        {
            _db = db;
        }

        public ActionResult Register(Guid? id)
        {
            var model = new RegisterViewModel();

            model.DatabasesInfo = _db.DatabaseInfo.OrderBy(x => x.Name).ToList();

            if (!id.HasValue)
                return PartialView(model);

            var job = _db.SqlCommandJob.FirstOrDefault(x => x.Id == id);

            if (job == null)
                return PartialView(model);

            model.Id = job.Id;
            model.Name = job.Name;
            model.Description = job.Description;
            model.Cron = job.Cron;
            model.Enabled = job.Enabled;
            model.DatabaseInfoId = job.DatabaseInfoId;
            model.Command = job.Command;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Save(RegisterModel model)
        {
            var job = model.Id.HasValue ? _db.SqlCommandJob.Find(model.Id) : new SqlCommandJob();

            if (job == null)
                return NotFound();

            var oldName = job.Name;

            job.Name = model.Name.Trim();
            job.Description = model.Description;
            job.Cron = model.Cron;
            job.Enabled = model.Enabled;
            job.DatabaseInfoId = model.DatabaseInfoId;
            job.Command = model.Command;

            if (!model.Id.HasValue)
                _db.Jobs.Add(job);

            _db.SaveChanges();

            if (!string.IsNullOrEmpty(oldName) && (oldName != job.Name || !job.Enabled))
                RecurringJob.RemoveIfExists(oldName);

            if (job.Enabled)
                RecurringJob.AddOrUpdate(job.Name, () => JobHelper.Execute(_db, job.Name), job.Cron, TimeZoneInfo.Local);

            return Json(new { job.Id });
        }
    }
}
