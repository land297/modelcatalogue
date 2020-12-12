using Aveva.Core.Database;
using Aveva.Core.Utilities.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue {
    public class PMLCommander {
        public static bool RunPMLCommandInParentheses(DbElement ce, string attribute, string command, out string error) {
            return RunPMLCommand(ce, attribute, $"({command})", out error);
        }
        public static  bool RunPMLCommand(DbElement ce, string attribute, string command, out string error) {
            if (string.IsNullOrWhiteSpace(command)) {
                error = "Command is empty";
                return false;
            }
            CurrentElement.Element = ce;
            Command pmlCommand;

            pmlCommand = Command.CreateCommand(attribute + " " + command);

            var result = pmlCommand.RunInPdms();
            error = pmlCommand.Error.MessageText();
            return result;
        }
        public static bool RunPMLCommand(string command) {
            if (string.IsNullOrWhiteSpace(command)) {
                return false;
            }
            Command pmlCommand;

            pmlCommand = Command.CreateCommand(command);

            var result = pmlCommand.RunInPdms();
            return result;
        }
    }
}
