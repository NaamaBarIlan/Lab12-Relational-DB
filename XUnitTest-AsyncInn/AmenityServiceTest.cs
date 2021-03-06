using Lab12_Relational_DB.Model;
using Lab12_Relational_DB.Model.Interfaces;
using Lab12_Relational_DB.Model.Services;
using System;
using Xunit;
using System.Threading.Tasks;
using Lab12_Relational_DB.Model.DTOs;
using System.Collections.Generic;

namespace XUnitTest_AsyncInn
{
    public class AmenityServiceTest : DatabaseTestBase
    {
        private IAmenity BuildRepository()
        {
            return new AmenityRepository(_db);
        }

        [Fact]
        public async Task CanSaveAndGetAmenity()
        {
            // Arrange

            var amenityDto = new AmenityDTO
            {
                ID = 22,
                Name = "TV"
            };

            var repository = BuildRepository();

            // Act
            AmenityDTO saved = await repository.Create(amenityDto);

            // Assert

            Assert.NotNull(saved);
            Assert.NotEqual(0, amenityDto.ID);
            Assert.Equal(saved.ID, amenityDto.ID);
            Assert.Equal(amenityDto.Name, saved.Name);
        }

        [Fact]
        public async Task CanGetASingleAmenity()
        {
            // Arrange

            AmenityDTO firstAmenityDto = new AmenityDTO
            {
                ID = 11,
                Name = "TV"
            };

            AmenityDTO secondAmenityDto = new AmenityDTO
            {
                ID = 12,
                Name = "AC"
            };

            var repository = BuildRepository();

            AmenityDTO saved1 = await repository.Create(firstAmenityDto);
            AmenityDTO saved2 = await repository.Create(secondAmenityDto);

            // Act

            AmenityDTO result1 = await repository.GetAmenity(1);
            AmenityDTO result2 = await repository.GetAmenity(2);

            // Assert

            Assert.Equal("TV", result1.Name);
            Assert.Equal("AC", result2.Name);
        }

        [Fact]
        public async Task CanGetAllAmenities()
        {
            // Arrange

            AmenityDTO firstAmenityDto = new AmenityDTO
            {
                ID = 11,
                Name = "TV"
            };

            AmenityDTO secondAmenityDto = new AmenityDTO
            {
                ID = 12,
                Name = "AC"
            };

            var repositoryAll = BuildRepository();

            AmenityDTO saved1 = await repositoryAll.Create(firstAmenityDto);
            AmenityDTO saved2 = await repositoryAll.Create(secondAmenityDto);

            // Act

            List<AmenityDTO> result = await repositoryAll.GetAmenities();
            

            // Assert

            Assert.Equal(5, result.Count);
        }

        [Fact]
        public async Task CanDeleteAnAmenity()
        {
            AmenityDTO firstAmenityDto = new AmenityDTO
            {
                ID = 11,
                Name = "Banana"
            };

            AmenityDTO secondAmenityDto = new AmenityDTO
            {
                ID = 12,
                Name = "Lemon"
            };

            AmenityDTO thirdAmenityDto = new AmenityDTO
            {
                ID = 13,
                Name = "Mango"
            };

            var repository = BuildRepository();

            AmenityDTO saved1 = await repository.Create(firstAmenityDto);
            AmenityDTO saved2 = await repository.Create(secondAmenityDto);
            AmenityDTO saved3 = await repository.Create(thirdAmenityDto);

            // Act

            List<AmenityDTO> resultBefore = await repository.GetAmenities();
            await repository.Delete(11);
            List<AmenityDTO> resultAfter = await repository.GetAmenities();

            // Assert

            Assert.Equal(5, resultAfter.Count);
        }

        [Fact]
        public async Task CanUpdateAnAmenity()
        {
            // Arrange

            AmenityDTO amenityDto = new AmenityDTO
            {
                ID = 1,
                Name = "mango"
            };

            var repository = BuildRepository();

            // Act

            await repository.Update(amenityDto);

            AmenityDTO result = await repository.GetAmenity(1);
            List<AmenityDTO> getAllAmenities = await repository.GetAmenities();

            // Assert
            Assert.Equal("mango", result.Name);
        }
    }
}
