using System;
using System.Collections.Generic;

public class Response
{
    public class Root 
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class Pivot
    {
        public int first_id { get; set; }
        public int second_id { get; set; }
    }

    public class Author
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string description { get; set; }
        public Pivot pivot { get; set; }
    }
    public class AuthorRoot : Root
    {
        public List<Author> data { get; set; }
    }

    public class Genre
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Pivot pivot { get; set; }
    }
    public class GenreRoot : Root
    {
        public List<Genre> data { get; set; }
    }

    public class Tag
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Pivot pivot { get; set; }
    }
    public class TagRoot : Root
    {
        public List<Tag> data { get; set; }
    }

    public class Auth
    {
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }

    public class Registr
    {
        public string token { get; set; }
        public string name { get; set; }
    }
    public class RegistrRoot : Root
    {
        public List<Registr> data { get; set; }
    }

    public class Story
    {
        public int id { get; set; }
        public string name_rus { get; set; }
        public string cover_link { get; set; }
    }
    public class StoryRoot : Root
    {
        public List<Story> data { get; set; }
    }
    public class MiddleStory
    {
        public string name_original { get; set; }
        public string name_rus { get; set; }
        public string description { get; set; }
        public int user_id { get; set; }
        public string cover_link { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int id { get; set; }
    }
    public class MiddleStoryRoot : Root
    {
        public MiddleStory data { get; set; }
    }
    public class MiddleStoryPivot : MiddleStory
    {
        public Pivot pivot { get; set; }
    }
    public class MiddleStoryPivotRoot : Root
    {
        public List<MiddleStoryPivot> data { get; set; }
    }
    public class FullStory
    {
        public int id { get; set; }
        public string name_original { get; set; }
        public string name_rus { get; set; }
        public string cover_link { get; set; }
        public string description { get; set; }
        public List<Author> authors { get; set; }
        public List<Tag> tags { get; set; }
        public List<Genre> genres { get; set; }
    }
    public class FullStoryRoot : Root
    {
        public List<FullStory> data { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class DataRoot : Root
    {
        public List<Data> data { get; set; }
    }

    public class ChapterAll
    {
        public int id { get; set; }
        public int story_id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int number { get; set; }
    }
    public class ChapterAllRoot : Root
    {
        public ChapterAll data { get; set; }
    }
    public class Chapter
    {
        public int id { get; set; }
        public string name { get; set; }
        public int number { get; set; }
    }
    public class ChapterRoot : Root
    {
        public List<Chapter> data { get; set; }
    }
    public class FulllChapter
    {
        public string name { get; set; }
        public int number { get; set; }
        public string text { get; set; }
        public int story_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int id { get; set; }
    }
    public class FulllChapterRoot : Root
    {
        public FulllChapter data { get; set; }
    }

    public class Mark
    {
        public int id { get; set; }
        public string name { get; set; }
        public int value { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
    }
    public class MarkRoot : Root
    {
        public List<Mark> data { get; set; }
    }
}
