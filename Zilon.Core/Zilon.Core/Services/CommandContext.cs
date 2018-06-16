namespace Zilon.Core.Services
{
    using System.Collections.Generic;

    using Zilon.Core.Tactics;

    struct CommandContext
    {
        private List<string> errors;

        public Sector Combat { get; set; }
        public string[] Errors => errors?.ToArray();

        public bool IsValid
        {
            get
            {
                return Errors == null || Errors.Length == 0;
            }
        }

        public void AddError(string message)
        {
            if (errors == null)
                errors = new List<string>();

            errors.Add(message);
        }
    }
}
