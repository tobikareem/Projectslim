USE [SlimWebDB]
GO
/****** Object:  Schema [slm]    Script Date: 11/17/2022 6:55:01 PM ******/
CREATE SCHEMA [slm]
GO
/****** Object:  Table [slm].[Category]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](max) NOT NULL,
	[CategoryDescription] [nvarchar](max) NOT NULL,
	[CategoryTags] [nvarchar](max) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[RazorPageId] [int] NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [slm].[Comment]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[Comment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[UserComment] [nvarchar](max) NOT NULL,
	[ProductId] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Comment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [slm].[Image]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[Image](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageId] [uniqueidentifier] NOT NULL,
	[UploadedImage] [varbinary](max) NOT NULL,
	[IsPrimaryImage] [bit] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [slm].[PageSection]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[PageSection](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RazorPageId] [int] NOT NULL,
	[PageSectionName] [varchar](100) NOT NULL,
	[Description] [varchar](500) NULL,
	[HasImage] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PageSection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [slm].[PageSectionResource]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[PageSectionResource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RazorPageId] [int] NOT NULL,
	[RazorPageSectionId] [int] NOT NULL,
	[ResourceActionId] [int] NOT NULL,
 CONSTRAINT [PK_PageSectionResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [slm].[Product]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RazorPageId] [int] NOT NULL,
	[ProductName] [nvarchar](max) NOT NULL,
	[ProductDescription] [nvarchar](max) NOT NULL,
	[StandardPrice] [decimal](18, 2) NOT NULL,
	[SalePrice] [decimal](18, 2) NOT NULL,
	[ProductTags] [nvarchar](max) NULL,
	[IsOnSale] [bit] NOT NULL,
	[IsNewProduct] [bit] NOT NULL,
	[IsTrending] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[CategoryId] [int] NOT NULL,
	[ProductQuantity] [int] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [slm].[ProductImage]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[ProductImage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[ImageId] [int] NOT NULL,
 CONSTRAINT [PK_ProductImage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [slm].[RazorPage]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[RazorPage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PageName] [varchar](100) NOT NULL,
	[Description] [varchar](500) NULL,
	[URL] [nvarchar](500) NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_RazorPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [slm].[RazorPageResourceActionMap]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[RazorPageResourceActionMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RazorPageId] [int] NOT NULL,
	[ResourceActionId] [int] NOT NULL,
 CONSTRAINT [PK_RazorPageResourceActionMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [slm].[ResourceAction]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[ResourceAction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceAction] [varchar](100) NOT NULL,
	[Description] [varchar](200) NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_ResourceAction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [slm].[Review]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[Review](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[UserReview] [nvarchar](max) NOT NULL,
	[ProductId] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[Pros] [nvarchar](max) NULL,
	[Cons] [nvarchar](max) NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [slm].[ShoppingCart]    Script Date: 11/17/2022 6:55:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [slm].[ShoppingCart](
	[Id] [nvarchar](450) NOT NULL,
	[CartUserId] [nvarchar](max) NOT NULL,
	[Quantity] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_ShoppingCart] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [slm].[Category] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[Category] ADD  DEFAULT ((0)) FOR [RazorPageId]
GO
ALTER TABLE [slm].[Comment] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[Image] ADD  DEFAULT ('2b1a1b4f-2d37-49a7-8fcd-029e016d7189') FOR [ImageId]
GO
ALTER TABLE [slm].[Image] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsPrimaryImage]
GO
ALTER TABLE [slm].[Image] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[PageSection] ADD  DEFAULT ((0)) FOR [HasImage]
GO
ALTER TABLE [slm].[PageSection] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[Product] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsOnSale]
GO
ALTER TABLE [slm].[Product] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsNewProduct]
GO
ALTER TABLE [slm].[Product] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsTrending]
GO
ALTER TABLE [slm].[Product] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[Product] ADD  DEFAULT ((0)) FOR [CategoryId]
GO
ALTER TABLE [slm].[Product] ADD  DEFAULT ((0)) FOR [ProductQuantity]
GO
ALTER TABLE [slm].[RazorPage] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[ResourceAction] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[Review] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[ShoppingCart] ADD  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [slm].[Category]  WITH CHECK ADD  CONSTRAINT [FK_Category_RazorPage] FOREIGN KEY([RazorPageId])
REFERENCES [slm].[RazorPage] ([Id])
GO
ALTER TABLE [slm].[Category] CHECK CONSTRAINT [FK_Category_RazorPage]
GO
ALTER TABLE [slm].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_Product] FOREIGN KEY([ProductId])
REFERENCES [slm].[Product] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [slm].[Comment] CHECK CONSTRAINT [FK_Comment_Product]
GO
ALTER TABLE [slm].[Image]  WITH CHECK ADD  CONSTRAINT [FK_Image_Product_ProductId] FOREIGN KEY([ProductId])
REFERENCES [slm].[Product] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [slm].[Image] CHECK CONSTRAINT [FK_Image_Product_ProductId]
GO
ALTER TABLE [slm].[PageSection]  WITH CHECK ADD  CONSTRAINT [FK_PageSection_PageId] FOREIGN KEY([RazorPageId])
REFERENCES [slm].[RazorPage] ([Id])
GO
ALTER TABLE [slm].[PageSection] CHECK CONSTRAINT [FK_PageSection_PageId]
GO
ALTER TABLE [slm].[PageSectionResource]  WITH CHECK ADD  CONSTRAINT [FK_PageSectionResource_PageId] FOREIGN KEY([RazorPageId])
REFERENCES [slm].[RazorPage] ([Id])
GO
ALTER TABLE [slm].[PageSectionResource] CHECK CONSTRAINT [FK_PageSectionResource_PageId]
GO
ALTER TABLE [slm].[PageSectionResource]  WITH CHECK ADD  CONSTRAINT [FK_PageSectionResource_ResourceActionId] FOREIGN KEY([ResourceActionId])
REFERENCES [slm].[ResourceAction] ([Id])
GO
ALTER TABLE [slm].[PageSectionResource] CHECK CONSTRAINT [FK_PageSectionResource_ResourceActionId]
GO
ALTER TABLE [slm].[PageSectionResource]  WITH CHECK ADD  CONSTRAINT [FK_PageSectionResource_SectionId] FOREIGN KEY([RazorPageSectionId])
REFERENCES [slm].[PageSection] ([Id])
GO
ALTER TABLE [slm].[PageSectionResource] CHECK CONSTRAINT [FK_PageSectionResource_SectionId]
GO
ALTER TABLE [slm].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Category] FOREIGN KEY([CategoryId])
REFERENCES [slm].[Category] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [slm].[Product] CHECK CONSTRAINT [FK_Product_Category]
GO
ALTER TABLE [slm].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_RazorPage] FOREIGN KEY([RazorPageId])
REFERENCES [slm].[RazorPage] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [slm].[Product] CHECK CONSTRAINT [FK_Product_RazorPage]
GO
ALTER TABLE [slm].[ProductImage]  WITH CHECK ADD  CONSTRAINT [FK_ProductImage_ImageImg] FOREIGN KEY([ImageId])
REFERENCES [slm].[Image] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [slm].[ProductImage] CHECK CONSTRAINT [FK_ProductImage_ImageImg]
GO
ALTER TABLE [slm].[ProductImage]  WITH CHECK ADD  CONSTRAINT [FK_ProductImage_ProductProd] FOREIGN KEY([ProductId])
REFERENCES [slm].[Product] ([Id])
GO
ALTER TABLE [slm].[ProductImage] CHECK CONSTRAINT [FK_ProductImage_ProductProd]
GO
ALTER TABLE [slm].[RazorPageResourceActionMap]  WITH CHECK ADD  CONSTRAINT [FK_RazorPageResourceActionMap_PageId] FOREIGN KEY([RazorPageId])
REFERENCES [slm].[RazorPage] ([Id])
GO
ALTER TABLE [slm].[RazorPageResourceActionMap] CHECK CONSTRAINT [FK_RazorPageResourceActionMap_PageId]
GO
ALTER TABLE [slm].[RazorPageResourceActionMap]  WITH CHECK ADD  CONSTRAINT [FK_RazorPageResourceActionMap_ResourceActionId] FOREIGN KEY([ResourceActionId])
REFERENCES [slm].[ResourceAction] ([Id])
GO
ALTER TABLE [slm].[RazorPageResourceActionMap] CHECK CONSTRAINT [FK_RazorPageResourceActionMap_ResourceActionId]
GO
ALTER TABLE [slm].[Review]  WITH CHECK ADD  CONSTRAINT [FK_Review_Product] FOREIGN KEY([ProductId])
REFERENCES [slm].[Product] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [slm].[Review] CHECK CONSTRAINT [FK_Review_Product]
GO
ALTER TABLE [slm].[ShoppingCart]  WITH CHECK ADD  CONSTRAINT [FK_ShoppingCart_Product_ProductId] FOREIGN KEY([ProductId])
REFERENCES [slm].[Product] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [slm].[ShoppingCart] CHECK CONSTRAINT [FK_ShoppingCart_Product_ProductId]
GO
