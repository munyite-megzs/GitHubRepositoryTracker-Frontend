﻿using GithubRepoTracker.Controllers;
using GithubRepoTracker.Data;
using GithubRepoTracker.Interfaces;
using GithubRepoTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GithubRepoTrackerTest.Controllers
{
    public class HomeControllerTests
    {

        private readonly Mock<RepoInterface> _mockRepoInterface;
        private readonly Mock<TopicInterface> _mockTopicInterface;
        private readonly Mock<LanguageInterface> _mockLanguageInterface;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockRepoInterface = new Mock<RepoInterface>();
            _mockTopicInterface = new Mock<TopicInterface>();
            _mockLanguageInterface = new Mock<LanguageInterface>();
            _controller = new HomeController(_mockRepoInterface.Object, _mockTopicInterface.Object, _mockLanguageInterface.Object);
        }

        [Fact]
        public void Index_ReturnsViewResultWithRipoListViewModel()
        {
            // Arrange
            var viewModel = new RepoListViewModel();

            // Act
            var result = _controller.Index(viewModel, null, null, null);

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsAssignableFrom<RepoListViewModel>(viewResult.ViewData.Model);
        }


        [Fact]
        public void Index_ReturnsViewResultWithViewModelWithRepos()
        {
            var viewModel = new RepoListViewModel();
            _mockRepoInterface.Setup(repo => repo.GetAllRepos().Result).Returns(MockData.GetTestRepoItems);

            // Act
            var result = _controller.Index(viewModel, null, null, null) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<RepoListViewModel>(result.ViewData.Model);
            var viewResultModel = result.ViewData.Model as RepoListViewModel;
            Assert.Equal(3, viewResultModel.Repos.Count());
        }

        [Fact]
        public void Index_ReturnsViewResultWithViewModelWithTopics()
        {
            var viewModel = new RepoListViewModel();
            _mockTopicInterface.Setup(topic => topic.GetAllTopics().Result).Returns(MockData.GetTestTopicItems);

            // Act
            var result = _controller.Index(viewModel, null, null, null) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<RepoListViewModel>(result.ViewData.Model);
            var viewResultModel = result.ViewData.Model as RepoListViewModel;
            Assert.Equal(5, viewResultModel.Topics.Count());
        }

        [Fact]
        public void Index_ReturnsViewResultWithViewModelWithLanguages()
        {
            var viewModel = new RepoListViewModel();
            _mockLanguageInterface.Setup(language => language.GetAllLanguages().Result).Returns(MockData.GetTestLanguageItems);

            // Act
            var result = _controller.Index(viewModel, null, null, null) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<RepoListViewModel>(result.ViewData.Model);
            var viewResultModel = result.ViewData.Model as RepoListViewModel;
            Assert.Equal(3, viewResultModel.Languages.Count());
        }


    }
}
