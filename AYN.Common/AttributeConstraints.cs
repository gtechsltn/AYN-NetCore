﻿namespace AYN.Common;

public static class AttributeConstraints
{
    // Category model
    public const int CategoryNameMinLength = 3;
    public const int CategoryNameMaxLength = 50;

    // Ad model
    public const int AdNameMinLength = 2;
    public const int AdNameMaxLength = 50;
    public const int DescriptionMinLength = 10;
    public const int DescriptionMaxLength = 5000;

    // Address model
    public const int AddressNameMaxLength = 50;

    // Post model
    public const int PostTitleMinLength = 3;
    public const int PostTitleMaxLength = 50;
    public const int PostContentMinLength = 5;
    public const int PostContentMaxLength = 1000;

    // Notification model
    public const int NotificationTextMinLength = 5;
    public const int NotificationTextMaxLength = 200;

    // Comment model
    public const int CommentContentMaxLength = 1000;

    // Contact model
    public const int FeedbackTitleMinLength = 3;
    public const int FeedbackTitleMaxLength = 50;

    public const int FeedbackContentMinLength = 10;
    public const int FeedbackContentMaxLength = 1000;

    // Message model
    public const int MessageContentMinLength = 1;
    public const int MessageContentMaxLength = 1000;


    // Report model
    public const int ReportContentMinLength = 20;
    public const int ReportContentMaxLength = 1000;

    // Tag model
    public const int TagNameMaxLength = 20;

    // Town model
    public const int TownNameMinLength = 3;
    public const int TownNameMaxLength = 50;

    // ApplicationUser Model
    public const int ApplicationUserUserNameMinLength = 3;
    public const int ApplicationUserUserNameMaxLength = 30;

    public const int ApplicationUserFirstNameMinLength = 3;
    public const int ApplicationUserFirstNameMaxLength = 30;

    public const int ApplicationUserMiddleNameMinLength = 3;
    public const int ApplicationUserMiddleNameMaxLength = 30;

    public const int ApplicationUserLastNameMinLength = 3;
    public const int ApplicationUserLastNameMaxLength = 30;

    public const int ApplicationUserAboutMinLength = 10;
    public const int ApplicationUserAboutMaxLength = 550;

    public const int ApplicationUserSocialContactUrlMaxLength = 25;

    public const int ApplicationUserBlockReasonMinLength = 5;
    public const int ApplicationUserBlockReasonMaxLength = 550;

    // WordBlacklist Model
    public const int BlacklistWordMaxLength = 25;
}
