using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CoriCore.Controllers;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;

namespace CoriCore.Tests.Controllers
{
    /// <summary>
    /// Unit tests for <see cref="AuthController"/>.  
    /// We cover *only* the actions that contain controller‑side branching (i.e., where the
    /// controller adds logic beyond merely forwarding the call to a service).
    /// </summary>
    public class AuthControllerTests
    {
        // -------------------------------------------------- helpers
        private static AuthController CreateController(
            out Mock<IAuthService> authMock,
            out Mock<IAdminService> adminMock)
        {
            authMock = new Mock<IAuthService>(MockBehavior.Strict);
            adminMock = new Mock<IAdminService>(MockBehavior.Strict);

            return new AuthController(authMock.Object, adminMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        private static User DummyUser(UserRole role = UserRole.Employee) => new()
        {
            UserId = 1,
            FullName = "Jane Doe",
            Email = "jane@example.com",
            Password = "hashed‑pwd",
            Role = role
        };

        // -------------------------------------------------- 1. Register
        [Fact]
        public async Task Register_Returns200_WhenUserCreated()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new UserEmailRegisterDTO { Email = "jane@example.com", Password = "pass" };

            auth.Setup(a => a.RegisterWithEmail(dto)).ReturnsAsync(DummyUser());

            var res = await controller.Register(dto) as OkObjectResult;
            Assert.NotNull(res);
            Assert.Equal(200, res!.StatusCode);
            Assert.Equal("User registered successfully", res.Value);
        }

        [Fact]
        public async Task Register_Returns400_WhenServiceReturnsNull()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new UserEmailRegisterDTO { Email = "exists@example.com", Password = "pass" };

            auth.Setup(a => a.RegisterWithEmail(dto)).ReturnsAsync((User?)null);

            var res = await controller.Register(dto) as BadRequestObjectResult;
            Assert.Equal(400, res!.StatusCode);
        }

        // -------------------------------------------------- 2. RegisterAdmin
        [Fact]
        public async Task RegisterAdmin_Returns200_AndCallsCreateAdmin()
        {
            var controller = CreateController(out var auth, out var admin);
            var dto = new UserEmailRegisterDTO { Email = "admin@example.com", Password = "pass" };
            var adminUser = DummyUser(UserRole.Admin);

            auth.Setup(a => a.RegisterWithEmail(It.Is<UserEmailRegisterDTO>(d => d.Role == UserRole.Admin)))
                .ReturnsAsync(adminUser);
            admin.Setup(a => a.CreateAdmin(It.IsAny<AdminDTO>())).ReturnsAsync("OK");

            var res = await controller.RegisterAdmin(dto) as OkObjectResult;
            Assert.Equal(200, res!.StatusCode);
            admin.Verify(a => a.CreateAdmin(It.Is<AdminDTO>(x => x.UserId == adminUser.UserId)), Times.Once);
        }

        [Fact]
        public async Task RegisterAdmin_Returns400_WhenUserNotCreated()
        {
            var controller = CreateController(out var auth, out var admin);
            var dto = new UserEmailRegisterDTO { Email = "admin@example.com", Password = "pass" };

            auth.Setup(a => a.RegisterWithEmail(It.IsAny<UserEmailRegisterDTO>())).ReturnsAsync((User?)null);

            var res = await controller.RegisterAdmin(dto) as BadRequestObjectResult;
            Assert.Equal(400, res!.StatusCode);
            admin.Verify(a => a.CreateAdmin(It.IsAny<AdminDTO>()), Times.Never);
        }

        // -------------------------------------------------- 3. GoogleRegister (employee)
        [Fact]
        public async Task GoogleRegister_Returns200_WhenServiceTrue()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new GoogleRegisterDTO { IdToken = "id", }; // other props not required

            auth.Setup(a => a.RegisterWithGoogle(dto.IdToken)).ReturnsAsync(true);

            var res = await controller.GoogleRegister(dto) as OkObjectResult;
            Assert.Equal(200, res!.StatusCode);
            Assert.Equal("Google registration successful.", res.Value);
        }

        [Fact]
        public async Task GoogleRegister_Returns400_WhenServiceFalse()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new GoogleRegisterDTO { IdToken = "id" };

            auth.Setup(a => a.RegisterWithGoogle(dto.IdToken)).ReturnsAsync(false);

            var res = await controller.GoogleRegister(dto) as BadRequestObjectResult;
            Assert.Equal(400, res!.StatusCode);
        }

        // -------------------------------------------------- 4. EmailLogin
        [Fact]
        public async Task EmailLogin_ReturnsJwt_WhenCredentialsValid()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new EmailLoginDTO { Email = "jane@example.com", Password = "pass" };
            const string jwt = "jwt‑token";

            auth.Setup(a => a.LoginWithEmail(dto.Email, dto.Password)).ReturnsAsync(jwt);

            var res = await controller.EmailLogin(dto) as OkObjectResult;
            // extract anonymous object via reflection
            var bodyType = res!.Value!.GetType();
            var tokenProp = bodyType.GetProperty("token");
            var messageProp = bodyType.GetProperty("message");
            string tokenVal = (string)tokenProp!.GetValue(res.Value)!;
            string messageVal = (string)messageProp!.GetValue(res.Value)!;

            Assert.Equal(200, res.StatusCode);
            Assert.Equal(jwt, tokenVal);
            Assert.Equal("Login successful", messageVal);
        }

        [Fact]
        public async Task EmailLogin_Returns401_WhenServiceThrows()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new EmailLoginDTO { Email = "jane@example.com", Password = "wrong" };

            auth.Setup(a => a.LoginWithEmail(dto.Email, dto.Password))
                .ThrowsAsync(new Exception("bad pwd"));

            var res = await controller.EmailLogin(dto) as UnauthorizedObjectResult;
            Assert.Equal(401, res!.StatusCode);
        }

        // -------------------------------------------------- 5. VerifyEmailCode
        [Fact]
        public async Task VerifyEmailCode_Returns200_WhenServiceTrue()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new VerifyEmailCodeDTO { Email = "jane@example.com", Code = "123456" };

            auth.Setup(a => a.VerifyEmailCodeAsync(dto)).ReturnsAsync(true);

            var res = await controller.VerifyEmailCode(dto) as OkObjectResult;
            Assert.Equal(200, res!.StatusCode);
            Assert.Equal("Email verified successfully", res.Value);
        }

        [Fact]
        public async Task VerifyEmailCode_Returns400_WhenServiceFalse()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new VerifyEmailCodeDTO { Email = "jane@example.com", Code = "bad" };

            auth.Setup(a => a.VerifyEmailCodeAsync(dto)).ReturnsAsync(false);

            var res = await controller.VerifyEmailCode(dto) as BadRequestObjectResult;
            Assert.Equal(400, res!.StatusCode);
        }

        // -------------------------------------------------- 6. GoogleLogin (generic) – just verify passthrough
        [Fact]
        public async Task GoogleLogin_ForwardsCode_AndPayload()
        {
            var controller = CreateController(out var auth, out _);
            var dto = new GoogleLoginDTO { IdToken = "id", Role = 0 };

            auth.Setup(a => a.LoginWithGoogle(dto.IdToken, dto.Role))
                .ReturnsAsync((409, "conflict", (string?)null));

            var res = await controller.GoogleLogin(dto) as ObjectResult;
            // anonymous object → reflection
            var bodyType = res!.Value!.GetType();
            var message = (string)bodyType.GetProperty("message")!.GetValue(res.Value)!;
            var token = bodyType.GetProperty("token")!.GetValue(res.Value);

            Assert.Equal(409, res.StatusCode);
            Assert.Equal("conflict", message);
            Assert.Null(token);
        }
    }
}
