﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Domain.Entities;

[Owned]
public class Address
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string PostCode { get; set; }
    public string Phone { get; set; }
}
