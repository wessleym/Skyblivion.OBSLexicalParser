using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class LockFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public LockFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5CodeChunkCollection codeChunks = new TES5CodeChunkCollection();
            if (functionArguments.Any())
            {
                int oblivionLockLevel = (int)functionArguments[0].Data;
                int skyrimLockLevel;
                if (oblivionLockLevel == 0) { skyrimLockLevel = 1; }
                else if (oblivionLockLevel > 0 && oblivionLockLevel < 99) { skyrimLockLevel = oblivionLockLevel; }
                else if (oblivionLockLevel == 99) { skyrimLockLevel = 100; }
                else if (oblivionLockLevel == 100) { skyrimLockLevel = 255; }
                else { throw new ConversionException("Oblivion lock level out of range (0-100):  " + oblivionLockLevel); }
                TES5ObjectCallArguments setLockLevelArguments = new TES5ObjectCallArguments()
                {
                    new TES5Integer(skyrimLockLevel)
                };
                codeChunks.Add(this.objectCallFactory.CreateObjectCall(calledOn, "SetLockLevel", setLockLevelArguments));
            }
            ITES4StringValue? lockAsOwnerBool = functionArguments.GetOrNull(1);
            TES5ObjectCallArguments lockArguments = new TES5ObjectCallArguments()
            {
                new TES5Bool(true),//abLock
                new TES5Bool(lockAsOwnerBool != null && (int)lockAsOwnerBool.Data == 1)//abAsOwner
            };
            codeChunks.Add(this.objectCallFactory.CreateObjectCall(calledOn, "Lock", lockArguments));
            return codeChunks;
        }
    }
}