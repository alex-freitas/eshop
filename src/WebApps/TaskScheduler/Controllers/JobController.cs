using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskScheduler.Entities;
using TaskScheduler.Infrastructure;
using TaskScheduler.Models;

namespace TaskScheduler.Controllers
{
    [GenerateClientActions]
    public class JobController : Controller
    {
        private readonly ILogger<JobController> _logger;
        private readonly TaskSchedulerDbContext _db;

        public JobController(ILogger<JobController> logger, TaskSchedulerDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public ActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult SearchResult(SearchModel model)
        {
            var query = ApplyFilters(model, _db.Jobs).AsQueryable();

            var jobs = query
                .OrderBy(model.SortBy)
                .Skip(model.StartRow - 1)
                .Take(model.MaxRows)
                .ToList();

            return PartialView(jobs);
        }

        private IEnumerable<Job> ApplyFilters(SearchModel model, IEnumerable<Job> jobs)
        {
            if (!string.IsNullOrEmpty(model.NamePart))
                jobs = jobs.Where(x => x.Name.Contains(model.NamePart));

            if (!string.IsNullOrEmpty(model.JobType))
                jobs = jobs.Where(x => x.JobType == model.JobType);

            if (model.Enabled.HasValue)
                jobs = jobs.Where(x => x.Enabled == model.Enabled.Value);

            return jobs;
        }

        public ActionResult SearchCount(SearchModel model)
        {
            var jobs = ApplyFilters(model, _db.Jobs);

            return Content(jobs.Count().ToString());
        }

        public ActionResult Register(int? id)
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var job = _db.Jobs.Find(id);

            if (job == null)
                return NotFound();

            _db.Jobs.Remove(job);

            _db.SaveChanges();

            RecurringJob.RemoveIfExists(job.Name);

            return Json(job);
        }
    }
}
