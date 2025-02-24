using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using Models;
using Newtonsoft.Json;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AppExtension
    {
        private readonly IConfiguration _cf;
        private readonly MailSetting _mailSettings;

        public AppExtension(IConfiguration cf, IOptions<MailSetting> mailOptions)
        {
            _cf = cf;
            _mailSettings = mailOptions.Value;
        }

        public string CreateHashPassword(string password)
        {
            using (var sha512 = SHA512.Create())
            {
                // Băm mật khẩu thành một mảng byte
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha512.ComputeHash(passwordBytes);

                // Chuyển đổi mảng byte thành chuỗi hex
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    builder.Append(b.ToString("X2"));
                }

                return builder.ToString();
            }
        }
        public string GeneratePassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789*&^%$#@";
            Random random = new Random();
            StringBuilder password = new StringBuilder();
            // Thêm một ký tự chữ thường, một ký tự chữ in hoa và một chữ số vào mật khẩu
            password.Append(validChars[random.Next(validChars.Length)]);
            password.Append(validChars[random.Next(26, 52)]); // Chữ in hoa
            password.Append(validChars[random.Next(52, 62)]); // Số

            int requiredLength = random.Next(8, 13);
            while (password.Length < requiredLength)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }
            return password.ToString();
        }
        public bool VerifyPasswordHash(string password, string passwordHash)
        {
            // Băm mật khẩu nhập vào
            string hashedPassword = CreateHashPassword(password);

            // So sánh mật khẩu băm tính toán được với mật khẩu đã lưu trữ
            return hashedPassword.Equals(passwordHash, StringComparison.OrdinalIgnoreCase);
        }
        public async Task<List<string>> UploadImagesToImgBB(List<IFormFile> files)
        {
            try
            {
                string apikey = _cf.GetValue<string>("ImgbbSettings:ApiKey");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", apikey);
                var imageUrlList = new List<string>();
                // List all of images
                foreach (var image in files)
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var content = new MultipartFormDataContent();
                        content.Add(new StreamContent(stream), "image", image.FileName);
                        var response = await client.PostAsync("https://api.imgbb.com/1/upload?key=" + apikey, content);
                        response.EnsureSuccessStatusCode();
                        var responseString = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonConvert.DeserializeObject<dynamic>(responseString);
                        var imageUrl = (string)jsonData.data.url;
                        imageUrlList.Add(imageUrl);
                    }
                }
                return imageUrlList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        //khi nao` co mail template roi` doi sau
        public async Task<IActionResult> SendMail(MailContent mailContent)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(mailContent.To));
                email.Subject = mailContent.Subject;

                //string projectDirectory = Directory.GetCurrentDirectory();
                //string OTPSamplePath = Path.Combine(projectDirectory, "wwwroot", "template.html");
                //string htmlContent = System.IO.File.ReadAllText(OTPSamplePath);
                //htmlContent = htmlContent.Replace("{Body}", mailContent.Body);
                //htmlContent = htmlContent.Replace("{OTP}", mailContent.OTP);
                var builder = new BodyBuilder();
                //builder.HtmlBody = htmlContent;
                builder.HtmlBody = mailContent.Body;
                email.Body = builder.ToMessageBody();

                // dùng SmtpClient của MailKit
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                try
                {
                    smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                    await smtp.SendAsync(email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                    // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                    System.IO.Directory.CreateDirectory("mailssave");
                    var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                    await email.WriteToAsync(emailsavefile);
                    throw new Exception(ex.ToString());
                }

                smtp.Disconnect(true);
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
        public string GenerateRandomOTP()
        {
            Random random = new Random();
            int otpValue = random.Next(0, 1000000); // Random số từ 0 đến 999999

            return otpValue.ToString("D6"); // Định dạng để luôn có 6 chữ số
        }
    }
}
