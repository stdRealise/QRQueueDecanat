using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class OperatorServiceConfiguration
    : IEntityTypeConfiguration<OperatorService>
{
    public void Configure(EntityTypeBuilder<OperatorService> builder)
    {
        builder.HasKey(operatorService => new
        {
            operatorService.OperatorId,
            operatorService.ServiceId
        });

        builder.HasOne(operatorService => operatorService.Operator)
            .WithMany(user => user.OperatorServices)
            .HasForeignKey(operatorService => operatorService.OperatorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(operatorService => operatorService.Service)
            .WithMany(service => service.OperatorServices)
            .HasForeignKey(operatorService => operatorService.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}