using System.Text.RegularExpressions;
using PIGI_PT_Application.Ports.Services;

namespace PIGI_PT_Infraestructure.ExternalServices.Sanitization
{
    public class DataSanitizerService : IDataSanitizerService
    {
        public string SanitizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var output = input;

            // Enmascarar correos electrónicos
            output = Regex.Replace(output, @"[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+", "[CORREO OCULTO]");

            // Enmascarar teléfonos (ejemplo genérico: 7 a 14 dígitos consecutivos o separados por guiones)
            output = Regex.Replace(output, @"(\b\d{3}[-.\s]??\d{3}[-.\s]??\d{4}\b)|(\b\d{7,14}\b)", "[TELÉFONO OCULTO]");

            // Enmascarar tarjetas de crédito (16 dígitos)
            output = Regex.Replace(output, @"\b(?:\d[ -]*?){13,16}\b", "[TARJETA OCULTA]");

            return output;
        }
    }
}
