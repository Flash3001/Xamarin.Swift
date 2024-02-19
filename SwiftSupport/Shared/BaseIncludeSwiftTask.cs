using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using Microsoft.Build.Framework;

namespace SwiftSupport.Shared
{
    public abstract class BaseIncludeSwiftTask : BaseTask
    {
        [Required]
        public string MtouchArch { get; set; }

        protected string GetLipoArgs(string[] availableArchs)
        {
            StringBuilder lipoCopyArgs = new StringBuilder();

            var arcs = MtouchArch.Split(',')
                                 .Select(c => c.Trim().ToLower())  // lipo uses lower case
                                 .ToList();

            var archsToRemove = availableArchs
                                    .Except(arcs)
                                    .Where(c => !string.IsNullOrWhiteSpace(c))
                                    .ToArray();

            if (archsToRemove.Length == 0)
            {
                lipoCopyArgs.Append(" -create");
            }
            else
            {
                foreach (var arch in archsToRemove)
                {
                    lipoCopyArgs.Append($" -remove {arch}");
                }
            }

            lipoCopyArgs.Append(" -output");

            return lipoCopyArgs.ToString();
        }
    }
}
