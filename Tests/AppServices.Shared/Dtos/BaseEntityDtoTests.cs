using GestaoPedidosPayment.AppServices.Shared.Dtos;
using Xunit;

namespace Tests.AppServices.Shared.Dtos
{
    public class BaseEntityDtoTests
    {
        [Fact(DisplayName = "BaseEntityDto should initialize with empty values")]
        public void BaseEntityDto_ShouldInitialize_WithEmptyValues()
        {
            // Arrange
            var baseEntityDto = new BaseEntityDto();

            // Assert
            Assert.Equal(Guid.Empty, baseEntityDto.Id); // Default value for a new GUID
            Assert.Equal(default(DateTime), baseEntityDto.CreatedAt); // Default DateTime value
        }

        [Fact(DisplayName = "BaseEntityDto should set properties correctly when assigned")]
        public void BaseEntityDto_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var expectedCreatedAt = DateTime.UtcNow;
            var baseEntityDto = new BaseEntityDto
            {
                Id = expectedId,
                CreatedAt = expectedCreatedAt
            };

            // Assert
            Assert.Equal(expectedId, baseEntityDto.Id);
            Assert.Equal(expectedCreatedAt, baseEntityDto.CreatedAt);
        }

        [Fact(DisplayName = "BaseEntityDto should assign default values when instantiated")]
        public void BaseEntityDto_ShouldAssignDefaultValues_WhenInstantiated()
        {
            // Arrange & Act
            var baseEntityDto = new BaseEntityDto();

            // Assert
            Assert.Equal(Guid.Empty, baseEntityDto.Id);
            Assert.Equal(default(DateTime), baseEntityDto.CreatedAt);
        }
    }
}
