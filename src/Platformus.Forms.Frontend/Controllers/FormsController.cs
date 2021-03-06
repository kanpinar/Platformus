﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ExtCore.Data.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Platformus.Configurations;
using Platformus.Forms.Data.Abstractions;
using Platformus.Forms.Data.Entities;

namespace Platformus.Forms.Frontend.Controllers
{
  [AllowAnonymous]
  public class FormsController : Platformus.Barebone.Frontend.Controllers.ControllerBase
  {
    private IConfigurationRoot configurationRoot;

    public FormsController(IStorage storage)
      : base(storage)
    {
      IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddStorage(storage);

      this.configurationRoot = configurationBuilder.Build();
    }

    [HttpPost]
    public IActionResult Send()
    {
      StringBuilder body = new StringBuilder();
      Dictionary<string, byte[]> attachments = new Dictionary<string, byte[]>();
      Form form = this.Storage.GetRepository<IFormRepository>().WithKey(int.Parse(this.Request.Form["formId"]));
      CompletedForm completedForm = new CompletedForm();

      completedForm.FormId = form.Id;
      completedForm.Created = DateTime.Now;
      this.Storage.GetRepository<ICompletedFormRepository>().Create(completedForm);
      this.Storage.Save();

      foreach (Field field in this.Storage.GetRepository<IFieldRepository>().FilteredByFormId(form.Id))
      {
        FieldType fieldType = this.Storage.GetRepository<IFieldTypeRepository>().WithKey(field.FieldTypeId);

        if (fieldType.Code == "FileUpload")
        {
          IFormFile file = this.Request.Form.Files[string.Format("field{0}", field.Id)];

          if (file != null && !string.IsNullOrEmpty(file.FileName))
          {
            string filename = file.FileName;

            if (!string.IsNullOrEmpty(filename) && filename.Contains("\\"))
              filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            attachments.Add(filename, this.GetBytesFromStream(file.OpenReadStream()));
          }
        }

        else
        {
          string value = this.Request.Form[string.Format("field{0}", field.Id)];

          body.AppendFormat("<p>{0}: {1}</p>", this.GetLocalizationValue(field.NameId), value);

          CompletedField completedField = new CompletedField();

          completedField.CompletedFormId = completedForm.Id;
          completedField.FieldId = field.Id;
          completedField.Value = value;
          this.Storage.GetRepository<ICompletedFieldRepository>().Create(completedField);
        }
      }

      this.Storage.Save();
      this.SendEmail(form, body.ToString(), attachments);

      string redirectUrl = form.RedirectUrl;

      if (this.configurationRoot["Globalization:SpecifyCultureInUrl"] == "yes")
        redirectUrl = "/" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + redirectUrl;

      return this.Redirect(redirectUrl);
    }

    // TODO: we shouldn't use MailKit directly, need to replace with the service instead
    private void SendEmail(Form form, string body, Dictionary<string, byte[]> attachments)
    {
      string smtpServer = this.configurationRoot["Email:SmtpServer"];
      string smtpPort = this.configurationRoot["Email:SmtpPort"];
      string smtpLogin = this.configurationRoot["Email:SmtpLogin"];
      string smtpPassword = this.configurationRoot["Email:SmtpPassword"];
      string smtpSenderEmail = this.configurationRoot["Email:SmtpSenderEmail"];
      string smtpSenderName = this.configurationRoot["Email:SmtpSenderName"];

      if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpPort) || string.IsNullOrEmpty(smtpLogin) || string.IsNullOrEmpty(smtpPassword))
        return;

      MimeMessage message = new MimeMessage();

      message.From.Add(new MailboxAddress(smtpSenderName, smtpSenderEmail));
      message.To.Add(new MailboxAddress(form.Email, form.Email));
      message.Subject = string.Format("{0} form data", this.GetLocalizationValue(form.NameId));

      BodyBuilder bodyBuilder = new BodyBuilder();

      bodyBuilder.HtmlBody = body;

      foreach (KeyValuePair<string, byte[]> attachment in attachments)
        bodyBuilder.Attachments.Add(attachment.Key, attachment.Value);

      message.Body = bodyBuilder.ToMessageBody();

      try
      {
        using (SmtpClient smtpClient = new SmtpClient())
        {
          smtpClient.Connect(smtpServer, int.Parse(smtpPort), false);
          smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
          smtpClient.Authenticate(smtpLogin, smtpPassword);
          smtpClient.Send(message);
          smtpClient.Disconnect(true);
        }
      }

      catch { }
    }

    private byte[] GetBytesFromStream(Stream input)
    {
      using (MemoryStream output = new MemoryStream())
      {
        input.CopyTo(output);
        return output.ToArray();
      }
    }
  }
}