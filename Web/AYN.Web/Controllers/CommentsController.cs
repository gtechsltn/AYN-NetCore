﻿using System.Threading.Tasks;

using AYN.Services.Data.Interfaces;
using AYN.Web.Infrastructure.Extensions;
using AYN.Web.ViewModels.Comments;
using Microsoft.AspNetCore.Mvc;

namespace AYN.Web.Controllers;

public class CommentsController : BaseController
{
    private readonly ICommentsService commentsService;
    private readonly IAdsService adsService;

    public CommentsController(
        ICommentsService commentsService,
        IAdsService adsService)
    {
        this.commentsService = commentsService;
        this.adsService = adsService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(string content, string adId)
    {
        if (!this.adsService.IsAdExisting(adId))
        {
            return this.Redirect("/");
        }

        await this.commentsService.Create(content, adId, this.User.GetId());
        return this.Redirect($"/Ads/Details?id={adId}");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id, string adId)
    {
        if (!this.adsService.IsAdExisting(adId))
        {
            return this.Redirect("/");
        }

        if (!this.commentsService.IsCommentExisting(id))
        {
            return this.Redirect($"/Ads/Details?id={adId}");
        }

        await this.commentsService.Delete(id);
        return this.Redirect($"/Ads/Details?id={adId}");
    }

    [HttpGet]
    public async Task<IActionResult> Vote(string vote, string commentId, string adId)
    {
        if (!this.adsService.IsAdExisting(adId))
        {
            return this.Redirect("/");
        }

        if (!this.commentsService.IsCommentExisting(commentId))
        {
            return this.Redirect($"/Ads/Details?id={adId}");
        }

        await this.commentsService.Vote(vote, commentId, this.User.GetId());

        return this.Redirect($"/Ads/Details?id={adId}");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string commentId, string adId)
    {
        if (!this.adsService.IsAdExisting(adId))
        {
            return this.Redirect("/");
        }

        if (!this.commentsService.IsCommentExisting(commentId))
        {
            return this.Redirect($"/Ads/Details?id={adId}");
        }

        var viewModel = await this.commentsService.GetByIdAsync<EditCommentInputModel>(commentId);
        return this.View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditCommentInputModel input, string adId)
    {
        if (this.adsService.IsAdExisting(adId))
        {
            return this.Redirect($"/Ads/Details?id={adId}");
        }

        await this.commentsService.EditAsync(input);
        return this.Redirect($"/Ads/Details?id={adId}");
    }
}
