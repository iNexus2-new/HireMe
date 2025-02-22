﻿namespace HireMe.Entities.Models
{
    using Ardalis.GuardClauses;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Input;
    using System;
    using System.Collections.Generic;

    public class Company : BaseModel
    {
        public string Title { get; set; }

        public string Email { get; set; }

        public bool isAuthentic_EIK { get; set; }

        public string About { get; set; }

        public bool Private { get; set; }

        public string Logo { get; set; }

        public int LocationId { get; set; }

        public string Adress { get; set; }

        public string PhoneNumber { get; set; }

        public string Website { get; set; }

        public double Rating { get; set; }
        public int RatingVotes { get; set; }
        public int VotedUsers { get; set; }

        public string PosterId { get; set; }

        public string Admin1_Id { get; set; }

        public string Admin2_Id { get; set; }

        public string Admin3_Id { get; set; }

        public int isApproved { get; set; }

        public DateTime Date { get; set; }

        public PromotionEnum Promotion { get; set; }

        public virtual ICollection<Jobs> Jobs { get; set; }

        public void Update(CreateCompanyInputModel viewModel, int approved, bool authenticEIK, User user)
        {
            Id = viewModel.Id;

            Guard.Against.NullOrEmpty(viewModel.Title, nameof(viewModel.Title));
            Title = viewModel.Title;

            Guard.Against.NullOrEmpty(viewModel.Email, nameof(viewModel.Email));
            Email = viewModel.Email;

            Guard.Against.NullOrEmpty(viewModel.About, nameof(viewModel.About));
            About = viewModel.About;

            Guard.Against.NullOrEmpty(viewModel.Adress, nameof(viewModel.Adress));
            Adress = viewModel.Adress;

            PhoneNumber = viewModel.PhoneNumber;
            Website = viewModel.Website;
            Admin1_Id = viewModel.Admin1_Id;
            Admin2_Id = viewModel.Admin2_Id;
            Admin3_Id = viewModel.Admin3_Id;
            Private = viewModel.Private;
            Logo = viewModel.Logo == null ? Logo : viewModel.Logo;
            LocationId = viewModel.LocationId;

            isAuthentic_EIK = authenticEIK;
            PosterId = user.Id;
            isApproved = approved;
            Date = DateTime.Now;
        }

    }


}