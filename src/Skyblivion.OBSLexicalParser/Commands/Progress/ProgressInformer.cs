/*//WTM:  Change:  No Longer Used

using Symfony.Component.Console.Output;

namespace Skyblivion.OBSLexicalParser.Commands.Progress
{
    class ProgressInformer
    {
        private int counter = 0;
        private OutputInterface output;
        public ProgressInformer(OutputInterface output)
        {
            this.output = output;
        }

        public void progress()
        {
            this.counter++;
            if (this.counter % 100 == 0)
            {
                this.output.writeln(this.counter." ..");
            }
        }
    }
}
*/