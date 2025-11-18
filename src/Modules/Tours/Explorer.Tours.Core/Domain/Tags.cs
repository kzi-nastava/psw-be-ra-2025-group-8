using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Tags : Entity
    {
        public string? Tag { get; set; }

        // ovo je klasa koja implementira tagove kad se prave preference u profilu turiste
        // trebalo bi da se ova klasa dalje koristi kada se npr tura pravi pa tagovi na turi

        public Tags() { } //prazan

        public Tags(string tag) //sa parametrima
        {
            Tag = tag;
        }
    }
}

