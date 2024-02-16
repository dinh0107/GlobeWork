using GlobeWork.DAL;
using GlobeWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GlobeWork.Utils
{
    public static class Utils
    {
        public static void EmployerLog(string content, EmployerLogType type, int employerId, decimal amount)
        {
            using (var unitOfwork = new UnitOfWork())
            {
                unitOfwork.EmployerLogRepository.Insert(new EmployerLog
                {
                    EmployerLogType = type,
                    Content = content,
                    UserId = employerId,
                    Amount = amount
                });
                unitOfwork.Save();
            }
        }
        public static string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                int index = random.Next(chars.Length);
                builder.Append(chars[index]);
            }
            return builder.ToString();
        }
        public static string DateCountDown(DateTime? sdate)
        {
            if (!sdate.HasValue)
            {
                return null;
            }

            TimeSpan timeSpan = sdate.Value - DateTime.Now;
            if (timeSpan.Days >= 1)
            {
                return $"{timeSpan.Days} ngày";
            }

            if (timeSpan.Days < 1 && timeSpan.Hours > 0)
            {
                return $"{timeSpan.Hours} giờ {timeSpan.Minutes} phút";
            }
            return $"{timeSpan.Minutes} phút";
        }
    }
}