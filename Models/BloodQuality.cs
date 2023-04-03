using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;

namespace FYP_Blood.Models
{
    [FunctionOutput]
    public class BloodQuality
    {
        [Parameter("uint256", "bloodqualityid", 1)]
        public BigInteger bloodqualityid { get; set; }

        [Parameter("uint256", "donorid", 2)]
        public BigInteger donorid { get; set; }

        [Parameter("uint256", "hosid", 3)]
        public BigInteger hosid { get; set; }

        [Parameter("string", "result", 4)]
        public string result { get; set; }

    }
}