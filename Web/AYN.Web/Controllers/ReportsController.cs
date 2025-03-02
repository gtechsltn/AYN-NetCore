﻿using System.Threading.Tasks;

using AYN.Services.Data.Interfaces;
using AYN.Web.Infrastructure.Extensions;
using AYN.Web.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;

namespace AYN.Web.Controllers;

public class ReportsController : BaseController
{
    private readonly IReportsService reportsService;

    public ReportsController(
        IReportsService reportsService)
    {
        this.reportsService = reportsService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReportInputModel input, string id, string reportedUser)
    {
        await this.reportsService.CreateAsync(input, id, reportedUser, this.User.GetId());

        return this.Redirect("/");
    }
}
