﻿using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace community_daily_diary_api.Dtos
{
    public record PostDto(ObjectId Id, string Title, string Body, int Vote, DateTime CreatedAt);
    public record CreatePostDto
        (
            [Required][MaxLength(250)] string Title, 
            [Required][MaxLength(2500)] string Body, 
            int Vote
        );
    public record UpdatePostDto
        (
            [Required][MaxLength(250)] string Title, 
            [Required][MaxLength(2500)] string Body, 
            int Vote
        );

    public record UpdatePostVoteDto
        (
            bool VoteUp
        );
}
