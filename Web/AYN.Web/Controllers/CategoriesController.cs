﻿using AYN.Services.Data.Interfaces;
using AYN.Web.ViewModels.SubCategories;
using Microsoft.AspNetCore.Mvc;

namespace AYN.Web.Controllers;

public class CategoriesController : BaseController
{
    private readonly ISubCategoriesService subCategoriesService;

    public CategoriesController(
        ISubCategoriesService subCategoriesService)
    {
        this.subCategoriesService = subCategoriesService;
    }

    [HttpGet]
    public IActionResult GetSubCategories(int id)
        => this.Json(this.subCategoriesService.GetAllByCategoryId<SubCategoryViewModel>(id));
}
