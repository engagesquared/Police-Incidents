﻿// <copyright file="ValidationException.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Exceptions
{
    using System;

    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {

        }
    }
}
