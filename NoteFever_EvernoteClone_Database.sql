use NoteFever_EvernoteClone
go

create table Label
(
    Id    int identity
        primary key,
    Title varchar(64) not null
        unique
)
go

create table Locale
(
    Id       int identity
        constraint Locale_pk
            primary key nonclustered,
    Locale   varchar(5),
    Language varchar(512) not null
)
go

create unique index Locale_Language_uindex
    on Locale (Locale)
go

create unique index Locale_Id_uindex
    on Locale (Id)
go

create unique index Locale_Language_uindex_2
    on Locale (Language)
go

create table Language
(
    Id          int identity
        unique,
    Language    varchar(5)  not null
        constraint Language_Locale_Locale_fk
            references Locale (Locale),
    Keyword     varchar(64) not null,
    Translation text        not null,
    constraint Language_pk
        primary key nonclustered (Language, Keyword)
)
go

create table NotebookLocation
(
    Id      int identity
        primary key,
    Path    varchar(max)  not null,
    Deleted bit default 0 not null
)
go

create index IX_NotebookLocation_Id
    on NotebookLocation (Id)
go

create table [User]
(
    Id              int identity
        primary key,
    Username        varchar(255)  not null
        unique,
    Password        varchar(255)  not null,
    FirstName       varchar(255),
    LastName        varchar(255),
    IsGoogleAccount bit default 0 not null,
    CreationDate    date          not null,
    LastLogin       datetime
)
go

create table LocationUser
(
    LocationID int not null
        constraint FK_LocationUser_NotebookLocation
            references NotebookLocation,
    UserID     int not null
        constraint FK_LocationUser_User
            references [User],
    primary key (LocationID, UserID)
)
go

create table Notebook
(
    Id           int identity
        primary key,
    UserID       int           not null
        constraint FK_Notebook_User
            references [User],
    LocationID   int           not null
        constraint FK_Notebook_NotebookLocation
            references NotebookLocation,
    Title        varchar(255)  not null,
    CreationDate date          not null,
    LastUpdated  datetime      not null,
    Deleted      bit default 0 not null
)
go

create table Note
(
    Id           int identity
        constraint PK__tmp_ms_x__3214EC07D4BBECAF
            primary key,
    NotebookID   int
        constraint FK_Note_Notebook
            references Notebook,
    Title        varchar(64)   not null,
    Content      varchar(max),
    Author       varchar(255)  not null,
    CreationDate date          not null,
    LastUpdated  datetime      not null,
    Deleted      bit default 0 not null
)
go

create index IX_Note_Id
    on Note (Id)
go

create table NoteHistory
(
    Id        int identity
        constraint PK__NoteHist__3214EC074D85FE0F
            primary key,
    NoteID    int          not null
        constraint FK_NoteHistory_Note
            references Note,
    Title     varchar(255) not null,
    Content   varchar(max),
    Timestamp timestamp    not null
)
go

create table NoteLabel
(
    NoteId  int not null
        constraint FK_NoteLabel_Note
            references Note,
    LabelId int not null
        constraint FK_NoteLabel_Label
            references Label,
    primary key (NoteId, LabelId)
)
go

create index IX_Table_ID
    on Notebook (Id)
go

create table Setting
(
    Id           int identity
        unique,
    UserID       int          not null
        constraint FK_Setting_ToUser
            references [User],
    Keyword      varchar(64)  not null,
    SettingValue varchar(255) not null,
    primary key (UserID, Keyword)
)
go

create index IX_Setting_Id
    on Setting (Id)
go

create table SharedNote
(
    NoteID int not null
        constraint SharedNote_Note_Id_fk
            references Note,
    UserID int not null
        constraint SharedNote_User_Id_fk
            references [User],
    constraint SharedNote_pk
        primary key nonclustered (NoteID, UserID)
)
go

create index IX_User_Id
    on [User] (Id)
go

