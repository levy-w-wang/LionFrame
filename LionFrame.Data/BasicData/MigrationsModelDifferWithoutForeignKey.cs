using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace LionFrame.Data.BasicData
{
    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<挂起>")]
    public class MigrationsModelDifferWithoutForeignKey : MigrationsModelDiffer
    {
        public MigrationsModelDifferWithoutForeignKey([NotNull]IRelationalTypeMappingSource typeMappingSource, [NotNull]IMigrationsAnnotationProvider migrationsAnnotations, [NotNull]IChangeDetector changeDetector, [NotNull]IUpdateAdapterFactory updateAdapterFactory, [NotNull]CommandBatchPreparerDependencies commandBatchPreparerDependencies) : base(typeMappingSource, migrationsAnnotations, changeDetector, updateAdapterFactory, commandBatchPreparerDependencies)
        {
        }

        public override IReadOnlyList<MigrationOperation> GetDifferences(IModel source, IModel target)
        {
            var operations = base.GetDifferences(source, target).Where(op => !(op is AddForeignKeyOperation)).Where(op => !(op is DropForeignKeyOperation)).ToList();

            foreach (var operation in operations.OfType<CreateTableOperation>())
                operation.ForeignKeys?.Clear();

            return operations;
        }
    }
}
