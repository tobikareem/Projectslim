-- drop table if exists slim.RazorPage
create table slim.RazorPage
(
	Id int identity(1,1) not null,
	PageName varchar(100) not null,
	[Description] varchar(500)	null,	
	[URL] nvarchar(500) null,		
	[Enabled] bit constraint df_razorpage_enabled default ((1)) not null,

	constraint PK_RazorPage primary key clustered (Id)
) 
go

-- drop table if exists slim.ResourceAction 
create table slim.ResourceAction 
(
	Id int           identity (1, 1) not null,
	ResourceAction   varchar (100) not null,
	[Description]    varchar (200) null,
	[Enabled]        bit           constraint df_resourceaction_enabled default ((1)) not null,
	CreatedBy   int           not null,
	CreatedDate       datetime      not null,
	ModifiedBy   int           null,
	ModifiedDate       datetime      null,

	constraint PK_ResourceAction primary key clustered (Id)
)
go

-- drop index if exists NC_slim_ResourceAction_ResourceAction_Enabled on slim.ResourceAction
create nonclustered index NC_slim_ResourceAction_ResourceAction_Enabled on slim.ResourceAction (ResourceAction, [Enabled])
	with (sort_in_tempdb=on, online=on)
go

-- drop index if exists NC_slim_ResourceAction_Enabled on slim.ResourceAction
create nonclustered index NC_slim_ResourceAction_Enabled on slim.ResourceAction ([Enabled])
	include (ResourceAction)
	with (sort_in_tempdb=on, online=on)
go

--drop table if exists slim.PageResourceActionMap
create table slim.RazorPageResourceActionMap
(
	Id int identity(1,1) not null,
	RazorPageId int not null,
	ResourceActionId int not null,

	constraint PK_PageResourceActionID primary key clustered (Id),
	constraint FK_RazorPageResourceActionMap_PageId foreign key (RazorPageId) references slim.RazorPage(Id),
	constraint FK_RazorPageResourceActionMap_ResourceActionId foreign key (ResourceActionId) references slim.ResourceAction(Id),
) 
go

-- drop index if exists NC_slim_PageResourceActionMap_PageID on slim.PageResourceActionMap
create nonclustered index NC_slim_PageResourceActionMap_PageID on slim.RazorPageResourceActionMap (RazorPageId)
	include (ResourceActionId)
	with (sort_in_tempdb=on, online=on)
go

-- drop index if exists NC_slim_PageResourceActionMap_ResourceActionID on slim.PageResourceActionMap
create nonclustered index NC_slim_PageResourceActionMap_ResourceActionID on slim.RazorPageResourceActionMap (ResourceActionId)
	include (RazorPageId)
	with (sort_in_tempdb=on, online=on)
go


-- drop table if exists slim.PageImage
create table slim.PageImage
(
	Id int identity(1,1) not null,
	PageImageName varchar(100) not null,
	[Description] varchar(500)	null,	
	[Image] nvarchar(500) not null,		
	[Enabled]        bit           constraint df_pageImage_enabled default ((1)) not null,
	CreatedBy   int           not null,
	CreatedDate       datetime      not null,
	ModifiedBy   int           null,
	ModifiedDate       datetime      null,

	constraint PK_PageImage primary key clustered (Id)
) 
go


create table slim.PageSection
(
	Id int identity(1,1) not null,
	RazorPageId int not null,
	PageSectionName varchar(100) not null,
	[Description] varchar(500)	null,	
	
	[Enabled]        bit           constraint df_pageSection_enabled default ((1)) not null,
	CreatedBy   int           not null,
	CreatedDate       datetime      not null,
	ModifiedBy   int           null,
	ModifiedDate       datetime      null,

	constraint PK_PageSection primary key clustered (Id),
	constraint FK_PageSection_PageId foreign key (RazorPageId) references slim.RazorPage(Id)
) 
go

create table slim.PageSectionResource
(
	Id int identity(1,1) not null,
	RazorPageId int not null,
	RazorPageSectionId int not null,
	ResourceActionId int not null,

	constraint PK_PageSectionResource primary key clustered (Id),
	constraint FK_PageSectionResource_PageId foreign key (RazorPageId) references slim.RazorPage(Id),
	constraint FK_PageSectionResource_SectionId foreign key (RazorPageSectionId) references slim.PageSection(Id),				
	constraint FK_PageSectionResource_ResourceActionId foreign key (ResourceActionId) references slim.ResourceAction(Id)
) 
go


create table slim.PageSectionImage
(
	Id int identity(1,1) not null,
	RazorPageId int not null,
	RazorPageSectionId int not null,
	PageImageId int not null,

	constraint PK_PageSectionImage primary key clustered (Id),
	constraint FK_PageSectionImage_PageId foreign key (RazorPageId) references slim.RazorPage(Id),
	constraint FK_PageSectionImage_SectionId foreign key (RazorPageSectionId) references slim.PageSection(Id),				
	constraint FK_PageSectionImage_PageImageId foreign key (PageImageId) references slim.PageImage(Id)
) 
go
