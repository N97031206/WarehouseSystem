﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace WarehouseSystem.Models.DbContextFactory
{
    public interface IDbContextFactory
    {
        DbContext GetDbContext();
    }
}
