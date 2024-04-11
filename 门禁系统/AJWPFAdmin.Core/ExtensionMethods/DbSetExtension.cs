using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.ExtensionMethods
{
    public static class DbSetExtension
    {
        public static void TryAttach<T>(this DbSet<T> dbSet, T value) where T : class
        {
			try
			{
				dbSet.Attach(value);
			}
			catch
			{
			}
        }
    }
}
