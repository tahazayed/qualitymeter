﻿using PagedList;
using QualityMeter.Core.Models;
using QualityMeter.Core.Services;
using QualityMeter.Infrastructure.Common.Services;
using QualityMeter.Infrastructure.Data;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace QualityMeter.Web.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationService _oApplicationService = new ApplicationService(new ApplicationsRepository(), new DebugLogger());
        private readonly QualityAttributesMetricService _oQualityAttributesMetricService = new QualityAttributesMetricService(new QualityAttributesMetricsRepository(), new DebugLogger());
        private readonly ApplicationEvaluationService _oApplicationEvaluationService = new ApplicationEvaluationService(new ApplicationEvaluationsRepository(), new DebugLogger());

        // GET: Applications
        public ActionResult Index(int page = 1)
        {
            return View(_oApplicationService.GetAll(sort: "Name").ToPagedList(page, 10));
        }

        // GET: Applications/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = _oApplicationService.GetById(id.Value);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // GET: Applications/Create
        public ActionResult Create()
        {
            Application application = new Application();
            return View(application);
        }

        // POST: Applications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Customer,CreationDate,LastUpdated,RowVersion")] Application application)
        {
            if (ModelState.IsValid)
            {
                application.Id = Guid.NewGuid();
                _oApplicationService.Add(application);
                foreach (var qualityAttributesMetric in _oQualityAttributesMetricService.GetAll())
                {
                    var applicationEvaluation = new ApplicationEvaluation
                    {
                        Id = Guid.NewGuid(),
                        QualityAttributesMetricId = qualityAttributesMetric.Id,
                        ApplicationId = application.Id,
                        UserValue = qualityAttributesMetric.EvaluationValue

                    };

                    _oApplicationEvaluationService.Add(applicationEvaluation);

                }

                return RedirectToAction("Edit", new { Id = application.Id });
            }

            return View(application);
        }

        // GET: Applications/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = _oApplicationService.GetById(id.Value);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Customer,CreationDate,LastUpdated,RowVersion")] Application application)
        {
            if (ModelState.IsValid)
            {
                application.LastUpdated = DateTime.Now;
                _oApplicationService.Update(application);
                return RedirectToAction("Index");
            }
            return View(application);
        }

        // GET: Applications/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = _oApplicationService.GetById(id.Value);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        public ActionResult DetailsReport(Guid? id)
        {
            return GetReport(id);
        }

        public ActionResult CriteriaSummaryReport(Guid? id)
        {
            return GetReport(id);
        }
        public ActionResult FactorsSummaryReport(Guid? id)
        {
            return GetReport(id);
        }

        private ActionResult GetReport(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = _oApplicationService.GetById(id.Value);
            if (application == null)
            {
                return HttpNotFound();
            }
            var lstApplicationEvaluation =
                _oApplicationEvaluationService.GetAll().Where(x => x.ApplicationId == id.Value);
            float SumQualityValue = 0, SumUserValue = 0;
            foreach (var applicationEvaluation in lstApplicationEvaluation)
            {
                SumQualityValue += applicationEvaluation.QualityValue;
                SumUserValue += applicationEvaluation.UserValue;
            }
            ViewBag.ApplicationName = application.Name;
            ViewBag.Customer = application.Customer;
            ViewBag.QualityPercentage = Math.Round(SumQualityValue * 100 / SumUserValue, 2);
            return View(lstApplicationEvaluation);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            _oApplicationService.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
