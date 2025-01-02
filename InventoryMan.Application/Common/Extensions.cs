namespace InventoryMan.Application.Common
{
    public static class Extensions
    {
        public static String FullMessageError(this Exception ex)
        {
            if (ex == null) return string.Empty;

            var messages = new List<string>();
            var currentException = ex;

            // Recorre todas las excepciones internas
            while (currentException != null)
            {
                messages.Add(currentException.Message);
                currentException = currentException.InnerException;
            }

            // Une todos los mensajes con un separador
            return string.Join(" -> ", messages);
        }
    }
}
