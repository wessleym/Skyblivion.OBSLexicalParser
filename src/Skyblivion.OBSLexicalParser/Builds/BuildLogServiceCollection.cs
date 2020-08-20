using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildLogServiceCollection : IDisposable
    {
        private readonly Lazy<MetadataLogService> metadataLogService;
        private readonly Lazy<MappedTargetsLogService> mappedTargetsLogService;
        public MetadataLogService MetadataLogService => metadataLogService.Value;
        public MappedTargetsLogService MappedTargetsLogService => mappedTargetsLogService.Value;
        private BuildLogServiceCollection(Build build)
        {
            MetadataLogService.DeleteFile(build);
            MappedTargetsLogService.DeleteFile(build);
            metadataLogService = new Lazy<MetadataLogService>(() => new MetadataLogService(build));
            mappedTargetsLogService = new Lazy<MappedTargetsLogService>(() => new MappedTargetsLogService(build));
        }

        public static BuildLogServiceCollection DeleteAndStartNewFiles(Build build)
        {
            return new BuildLogServiceCollection(build);
        }

        public void Dispose()
        {
            if (metadataLogService.IsValueCreated) { metadataLogService.Value.Dispose(); }
            if (mappedTargetsLogService.IsValueCreated) { mappedTargetsLogService.Value.Dispose(); }
        }
    }
}
