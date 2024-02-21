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
        public static void UserLog(string content,bool status, int userId)
        {
            using (var unitOfwork = new UnitOfWork())
            {
                unitOfwork.UserLogRepository.Insert(new UserLog
                {
                    Content = content,
                    UserId = userId,
                    Status = status
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
        public static string DateVn(DateTime? sdate)
        {
            if (!sdate.HasValue)
            {
                return null;
            }
            DateTime value = sdate.Value;
            DateTime now = DateTime.Now;
            if (now.Day - value.Day == 1 && value.Month == now.Month && value.Year == now.Year)
            {
                return $"Hôm qua, lúc {value:HH:mm}";
            }

            if (value.Day != now.Day || value.Month != now.Month || value.Year != now.Year)
            {
                return value.ToString("dd/MM/yyyy");
            }
            TimeSpan timeSpan = now - value;
            int minutes = timeSpan.Minutes;
            int hours = timeSpan.Hours;
            if (hours < 1)
            {
                return $"{minutes} phút trước";
            }

            return $"{hours} giờ {minutes} phút trước";
        }
    }
}