using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intrastructure.Repositories
{
    public static class IconHelper
    {
        public static string LoadSvgContent(string iconName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resourceName = asm.GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith($".Icons.{iconName}.svg", StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                throw new FileNotFoundException($"No se encontró el recurso incrustado: {iconName}.svg");

            using var stream = asm.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
