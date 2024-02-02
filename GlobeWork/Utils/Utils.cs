using GlobeWork.DAL;
using GlobeWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}