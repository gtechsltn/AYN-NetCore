﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AYN.Services.Data;
using AYN.Services.Data.Interfaces;
using AYN.Web.ViewModels.Ads;
using AYN.Web.ViewModels.Categories;
using AYN.Web.ViewModels.SubCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AYN.Web.Controllers
{
    [Authorize]
    public class AdsController : Controller
    {
        private readonly ICategoriesService categoriesService;
        private readonly ITownsService townsService;
        private readonly IAdsService adsService;
        private readonly IWebHostEnvironment environment;
        private readonly ISubCategoriesService subCategoriesService;

        public AdsController(
            ICategoriesService categoriesService,
            ITownsService townsService,
            IAdsService adsService,
            IWebHostEnvironment environment,
            ISubCategoriesService subCategoriesService)
        {
            this.categoriesService = categoriesService;
            this.townsService = townsService;
            this.adsService = adsService;
            this.environment = environment;
            this.subCategoriesService = subCategoriesService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateAdInputModel()
            {
                Categories = await this.categoriesService.GetAllAsKeyValuePairsAsync(),
                Towns = await this.townsService.GetAllAsKeyValuePairsAsync(),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdInputModel input)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await this.adsService.CreateAsync(input, userId, this.environment.WebRootPath);
            }
            catch (Exception e)
            {
                input.Categories = await this.categoriesService.GetAllAsKeyValuePairsAsync();
                input.Towns = await this.townsService.GetAllAsKeyValuePairsAsync();

                this.ModelState.AddModelError(string.Empty, e.Message);

                return this.View(input);
            }

            return this.Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> All(
            string search,
            string town,
            int? categoryId,
            string orderBy = "createdOnDesc",
            int id = 1)
        {
            try
            {
                var ads = await this.adsService
                    .GetAllAsync<GetAdsViewModel>(search, orderBy, categoryId);

                var itemsPerPage = 12;

                var viewModel = new ListAllAdsViewModel()
                {
                    Count = ads.Count(),
                    ItemsPerPage = itemsPerPage,
                    AllFromSearch = ads.Skip((id - 1) * itemsPerPage).Take(itemsPerPage),
                    PageNumber = id,
                    OrderBy = orderBy,
                    Town = town,
                    CategoryId = categoryId,
                    Search = search,
                    TotalResults = ads.Count(),
                    AllCategoriesWithAllSubCategories = new Dictionary<CategoryViewModel, List<SubCategoryViewModel>>(),
                };

                var categories = this.categoriesService
                    .GetAll<CategoryViewModel>();

                foreach (var category in categories)
                {
                    var subCategories = await this.subCategoriesService
                        .GetAllByCategoryId<SubCategoryViewModel>(category.Id)
                        .ToListAsync();

                    var categoryViewModel = new CategoryViewModel()
                    {
                        Id = category.Id,
                        Name = category.Name,
                        PictureExtension = category.PictureExtension,
                    };

                    viewModel.AllCategoriesWithAllSubCategories
                        .Add(categoryViewModel, subCategories);
                }

                return this.View(viewModel);
            }
            catch (Exception e)
            {
                return this.View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var viewModel = await this.adsService.GetDetails<GetDetailsViewModel>(id);
            return this.View(viewModel);
        }
    }
}
