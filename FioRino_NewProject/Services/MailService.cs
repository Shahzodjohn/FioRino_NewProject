using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly FioRinoBaseContext _context;

        public MailService(IOptions<MailSettings> settings, FioRinoBaseContext context)
        {
            _settings = settings.Value;
            _context = context;
        }

        public async Task SendEmailAsync(MailRequestDTO mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_settings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));

            var builder = new BodyBuilder();
            #region
            //if (mailRequest.Attechments != null)
            //{
            //    byte[] filebytes;
            //    foreach (var file in mailRequest.Attechments)
            //    {
            //        if (file.Length > 0)
            //        {
            //            using (var ms = new MemoryStream())
            //            {
            //                file.CopyTo(ms);
            //                filebytes = ms.ToArray();
            //            }
            //            builder.Attachments.Add(file.FileName, filebytes, ContentType.Parse(file.ContentType));
            //        }
            //    }
            //}
            #endregion
            var random = new Random();
            var randomNumber = random.Next(1000, 9999);
            var ToUser = email.To.ToString();
            // var userCode = await _context.DmUsers.FirstOrDefaultAsync(x=>x.Email == t)
            var compare = await _context.DmUsers.FirstOrDefaultAsync(x => x.Email == ToUser);
            var UserCode = (from c in _context.DmCodesForResetPasswords
                            join u in _context.DmUsers on c.UserId equals u.Id
                            where c.UserId == compare.Id
                            select c).FirstOrDefault();

            var findIDs = await _context.DmCodesForResetPasswords.FirstOrDefaultAsync(x => x.UserId == compare.Id);

            var date = DateTime.Now.AddHours(2);
            if (compare != null)
            {
                if (UserCode != null)
                {
                    UserCode.RandomNumber = randomNumber.ToString();
                }
                else
                {
                    var dataInsert = await _context.DmCodesForResetPasswords.AddAsync(new DmCodesForResetPassword
                    {
                        RandomNumber = randomNumber.ToString(),
                        UserId = compare.Id,
                        ValidDate = date
                    });
                }
                email.Subject = "Twój Identyfikator odzyskiwania hasła";
                builder.HtmlBody = "Drogi Użytkowniku! Poprosiłeś o odzyskanie hasła. Kod odzyskiwania - " + randomNumber.ToString();
                email.Body = builder.ToMessageBody();
                var codeCompare = await _context.DmCodesForResetPasswords.FirstOrDefaultAsync(x => x.RandomNumber == randomNumber.ToString());

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_settings.Mail, _settings.Password);
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
                await _context.SaveChangesAsync();
            }

            //builder.HtmlBody = mailRequest.Body;
            //  builder.HtmlBody = string.Join(", ", gtin); // вот так вот надо делать Tostring(); //body
        }
    }
}
