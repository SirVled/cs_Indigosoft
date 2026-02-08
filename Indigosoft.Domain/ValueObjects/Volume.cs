using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Domain.ValueObjects
{
    public readonly record struct Volume(decimal Value)
    {
        public static implicit operator decimal(Volume volume) => volume.Value;
        public static implicit operator Volume(decimal value) => new(value);
    }
}
