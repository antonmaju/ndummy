﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dummy.Factories
{
    public interface IFactory
    {
        object Create();
    }

    public interface IFactory<out T> : IFactory
    {
        new T Create();
    }
}