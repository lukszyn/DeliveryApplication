using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Serializers;
using DeliveryApp.BusinessLayer.Services;
using DeliveryApp.DataLayer.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DeliveryApp.Tests
{
    public class WaybillsServiceTests
    {
        [Test]
        public void GetLocation_ProvideValidAddress_ReturnsGeoCoordinates()
        {
            var address = new Address()
            {
                Street = "aleja Grunwaldzka",
                Number = 26,
                City = "Gda�sk",
            };

            var pckgService = new Mock<IPackagesService>();
            var vhService = new Mock<IVehiclesService>();
            var usrService = new Mock<IUsersService>();
            var serializer = new Mock<ISerializer>();

            IWaybillsService wbService = new WaybillsService(pckgService.Object,
                usrService.Object, vhService.Object, serializer.Object);

            var coordinates = wbService.GetLocation(address);

            coordinates.Latitude.Should().BeApproximately(54.3754, 0.001);
            coordinates.Longitude.Should().BeApproximately(18.6159, 0.001);
        }

        [Test]
        public void ChooseDriver_ProvideValidData_ReturnsClosestDriver()
        {
            var sender = new User()
            {
                Email = "anowak@gmail.com",
                FirstName = "Anna",
                LastName = "Nowak",
                Address = new Address()
                {
                    Street = "Chylo�ska",
                    Number = 84,
                    City = "Gdynia"
                },
                UserType = UserType.Customer
            };

            var driver1 = new User()
            {
                Email = "jankowalski@gmail.com",
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = new Address()
                {
                    Street = "Olszty�ska",
                    Number = 18,
                    City = "Gda�sk"
                },
                UserType = UserType.Driver,
                Vehicle = new Vehicle()
                {
                    Capacity = 100,
                    Occupancy = 0
                }
            };

            var driver2 = new User()
            {
                Email = "krzysztofnowak@gmail.com",
                FirstName = "Krzysztof",
                LastName = "Nowak",
                Address = new Address()
                {
                    Street = "Jagiello�ska",
                    Number = 18,
                    City = "Olsztyn"
                },
                UserType = UserType.Driver,
                Vehicle = new Vehicle()
                {
                    Capacity = 100,
                    Occupancy = 0
                }
            };

            Package package = new Package()
            {
                Number = Guid.NewGuid(),
                Sender = sender,
                ReceiverAddress = new Address()
                {
                    Street = "Piastowska",
                    Number = 60,
                    City = "Gda�sk"
                },
                Size = Size.SMALL,
                Status = Status.PENDING_SENDING
            };

            var pckgService = new Mock<IPackagesService>();
            var vhService = new Mock<IVehiclesService>();
            var usrService = new Mock<IUsersService>();
            var serializer = new Mock<ISerializer>();

            IWaybillsService wbService = new WaybillsService(pckgService.Object,
                usrService.Object, vhService.Object, serializer.Object);

            var closestDriver = wbService.ChooseDriver(package, new List<User>
            {
                driver1,
                driver2
            });

            closestDriver.Should().BeSameAs(driver1);
        }

        [Test]
        public void ChooseDriver_ClosestDriverIsOverLoaded_ReturnsSecondClosestDriver()
        {
            var sender = new User()
            {
                Email = "anowak@gmail.com",
                FirstName = "Anna",
                LastName = "Nowak",
                Address = new Address()
                {
                    Street = "Chylo�ska",
                    Number = 84,
                    City = "Gdynia"
                },
                UserType = UserType.Customer
            };

            var driver1 = new User()
            {
                Email = "jankowalski@gmail.com",
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = new Address()
                {
                    Street = "Olszty�ska",
                    Number = 18,
                    City = "Gda�sk"
                },
                UserType = UserType.Driver,
                Vehicle = new Vehicle()
                {
                    Capacity = 100,
                    Occupancy = 95
                }
            };

            var driver2 = new User()
            {
                Email = "krzysztofnowak@gmail.com",
                FirstName = "Krzysztof",
                LastName = "Nowak",
                Address = new Address()
                {
                    Street = "Jagiello�ska",
                    Number = 18,
                    City = "Olsztyn"
                },
                UserType = UserType.Driver,
                Vehicle = new Vehicle()
                {
                    Capacity = 100,
                    Occupancy = 0
                }
            };

            Package package = new Package()
            {
                Number = Guid.NewGuid(),
                Sender = sender,
                ReceiverAddress = new Address()
                {
                    Street = "Piastowska",
                    Number = 60,
                    City = "Gda�sk"
                },
                Size = Size.SMALL,
                Status = Status.PENDING_SENDING
            };

            var pckgService = new Mock<IPackagesService>();
            var vhService = new Mock<IVehiclesService>();
            var usrService = new Mock<IUsersService>();
            var serializer = new Mock<ISerializer>();

            IWaybillsService wbService = new WaybillsService(pckgService.Object,
                usrService.Object, vhService.Object, serializer.Object);

            var closestDriver = wbService.ChooseDriver(package, new List<User>
            {
                driver1,
                driver2
            });

            closestDriver.Should().BeSameAs(driver2);
        }

        [Test]
        public void MatchPackages_TwoPackages_ReturnsOnePackagePerDriver()
        {
            var sender1 = new User()
            {
                Email = "anowak@gmail.com",
                FirstName = "Anna",
                LastName = "Nowak",
                Address = new Address()
                {
                    Street = "Chylo�ska",
                    Number = 84,
                    City = "Gdynia"
                },
                UserType = UserType.Customer
            };

            var sender2 = new User()
            {
                Email = "kmalinowska@gmail.com",
                FirstName = "Katarzyna",
                LastName = "Malinowska",
                Address = new Address()
                {
                    Street = "Dworcowa",
                    Number = 70,
                    City = "Olsztyn"
                },
                UserType = UserType.Customer
            };

            var driver1 = new User()
            {
                Email = "jankowalski@gmail.com",
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = new Address()
                {
                    Street = "Olszty�ska",
                    Number = 18,
                    City = "Gda�sk"
                },
                UserType = UserType.Driver,
                Vehicle = new Vehicle()
                {
                    Capacity = 100,
                    Occupancy = 0
                },
                Packages = new List<Package>()
            };

            var driver2 = new User()
            {
                Email = "krzysztofnowak@gmail.com",
                FirstName = "Krzysztof",
                LastName = "Nowak",
                Address = new Address()
                {
                    Street = "Jagiello�ska",
                    Number = 18,
                    City = "Olsztyn"
                },
                UserType = UserType.Driver,
                Vehicle = new Vehicle()
                {
                    Capacity = 100,
                    Occupancy = 0
                },
                Packages = new List<Package>()
            };

            Package package1 = new Package()
            {
                Number = Guid.NewGuid(),
                Sender = sender1,
                ReceiverAddress = new Address()
                {
                    Street = "Piastowska",
                    Number = 60,
                    City = "Gda�sk"
                },
                Size = Size.SMALL,
                Status = Status.PENDING_SENDING
            };

            Package package2 = new Package()
            {
                Number = Guid.NewGuid(),
                Sender = sender2,
                ReceiverAddress = new Address()
                {
                    Street = "Traktorowa",
                    Number = 13,
                    City = "Olsztyn"
                },
                Size = Size.MEDIUM,
                Status = Status.PENDING_SENDING
            };

            var pckgService = new Mock<IPackagesService>();
            var vhService = new Mock<IVehiclesService>();
            var usrService = new Mock<IUsersService>();
            var serializer = new Mock<ISerializer>();

            pckgService.Setup(s => s.GetAllPackagesToBeSend())
                .Returns(new List<Package>
                {
                    package1,
                    package2,
                });

            usrService.Setup(u => u.GetAllDrivers()).Returns(new List<User>
                {
                    driver1,
                    driver2,
                });

            IWaybillsService wbService = new WaybillsService(pckgService.Object,
                usrService.Object, vhService.Object, serializer.Object);

            wbService.MatchPackages();

            driver1.Packages.Count.Should().Be(1);
            driver2.Packages.Count.Should().Be(1);
        }

        [Test]
        public void MatchPackages_OverloadedDrivers_PackagesNotMatched()
        {
            var sender1 = new User()
            {
                Email = "anowak@gmail.com",
                FirstName = "Anna",
                LastName = "Nowak",
                Address = new Address()
                {
                    Street = "Chylo�ska",
                    Number = 84,
                    City = "Gdynia"
                },
                UserType = UserType.Customer
            };

            var sender2 = new User()
            {
                Email = "kmalinowska@gmail.com",
                FirstName = "Katarzyna",
                LastName = "Malinowska",
                Address = new Address()
                {
                    Street = "Dworcowa",
                    Number = 70,
                    City = "Olsztyn"
                },
                UserType = UserType.Customer
            };

            var driver1 = new User()
            {
                Email = "jankowalski@gmail.com",
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = new Address()
                {
                    Street = "Olszty�ska",
                    Number = 18,
                    City = "Gda�sk"
                },
                UserType = UserType.Driver,
                Vehicle = new Vehicle()
                {
                    Capacity = 100,
                    Occupancy = 95
                },
                Packages = new List<Package>()
            };

            var driver2 = new User()
            {
                Email = "krzysztofnowak@gmail.com",
                FirstName = "Krzysztof",
                LastName = "Nowak",
                Address = new Address()
                {
                    Street = "Jagiello�ska",
                    Number = 18,
                    City = "Olsztyn"
                },
                UserType = UserType.Driver,
                Vehicle = new Vehicle()
                {
                    Capacity = 100,
                    Occupancy = 95
                },
                Packages = new List<Package>()
            };

            Package package1 = new Package()
            {
                Number = Guid.NewGuid(),
                Sender = sender1,
                ReceiverAddress = new Address()
                {
                    Street = "Piastowska",
                    Number = 60,
                    City = "Gda�sk"
                },
                Size = Size.SMALL,
                Status = Status.PENDING_SENDING
            };

            Package package2 = new Package()
            {
                Number = Guid.NewGuid(),
                Sender = sender2,
                ReceiverAddress = new Address()
                {
                    Street = "Traktorowa",
                    Number = 13,
                    City = "Olsztyn"
                },
                Size = Size.MEDIUM,
                Status = Status.PENDING_SENDING
            };

            var pckgService = new Mock<IPackagesService>();
            var vhService = new Mock<IVehiclesService>();
            var usrService = new Mock<IUsersService>();
            var serializer = new Mock<ISerializer>();

            pckgService.Setup(s => s.GetAllPackagesToBeSend())
                .Returns(new List<Package>
                {
                    package1,
                    package2,
                });

            usrService.Setup(u => u.GetAllDrivers()).Returns(new List<User>
                {
                    driver1,
                    driver2,
                });

            IWaybillsService wbService = new WaybillsService(pckgService.Object,
                usrService.Object, vhService.Object, serializer.Object);

            wbService.MatchPackages();

            package1.Status.Should().Be(Status.PENDING_SENDING);
            package2.Status.Should().Be(Status.PENDING_SENDING);
        }
    }
}