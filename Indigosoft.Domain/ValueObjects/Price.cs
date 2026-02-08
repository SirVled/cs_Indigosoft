using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Domain.ValueObjects
{
    public readonly record struct Price(decimal Value)
    {
        public static implicit operator decimal(Price price) => price.Value;
        public static implicit operator Price(decimal value) => new(value);
    }
}
