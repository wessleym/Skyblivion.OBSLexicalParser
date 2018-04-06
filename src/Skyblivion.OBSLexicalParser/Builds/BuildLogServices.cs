using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildLogServices : IDisposable
    {
        private Lazy<MetadataLogService> metadataLogService;
        private Lazy<MappedTargetsLogService> mappedTargetsLogService;
        public MetadataLogService MetadataLogService => metadataLogService.Value;
        public MappedTargetsLogService MappedTargetsLogService => mappedTargetsLogService.Value;
        public BuildLogServices(Build build)
        {
            MetadataLogService.DeleteFile(build);
            MappedTargetsLogService.DeleteFile(build);
            metadataLogService = new Lazy<MetadataLogService>(() => new MetadataLogService(build));
            mappedTargetsLogService = new Lazy<MappedTargetsLogService>(() => new MappedTargetsLogService(build));
        }

        public void Dispose()
        {
            if (metadataLogService.IsValueCreated) { metadataLogService.Value.Dispose(); }
            if (mappedTargetsLogService.IsValueCreated) { mappedTargetsLogService.Value.Dispose(); }
        }
    }
}
