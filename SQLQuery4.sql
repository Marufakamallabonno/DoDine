use Doc&Dine;
use DocnDine;
CREATE TABLE [User] (
  UserId   int NOT NULL IDENTITY(1001,1) PRIMARY KEY, 
  UserName varchar(100) NOT NULL, 
  Email    varchar(100) NOT NULL, 
  Password varchar(20) NOT NULL, 
  PhoneNo  varchar(11) NOT NULL, 
  Picture  varchar(500) 
  
);
select * from [User];
truncate table [User];

CREATE TABLE Blogger (
  BloggerId       int NOT NULL IDENTITY(2001,1) PRIMARY KEY , 
  UserId int FOREIGN KEY REFERENCES [User](UserId) NOT NULL,
  Location        varchar(255), 
  Story           varchar(500), 
  YearsofBlogging int, 
  Education       varchar(255), 
  Age             int, 
  SavoryOrSweet   varchar(50)
);

select * from Blogger;
truncate table Blogger;

CREATE TABLE HomeChef
(
    HomeChefId int NOT NULL IDENTITY(3001,1) PRIMARY KEY,
    UserId int NOT NULL FOREIGN KEY REFERENCES [User](UserId),
    FbLink varchar(500),
    InstaLink varchar(500),
    TwitterLink varchar(500),
    Address  varchar(500),
	Area varchar(50) NOT NULL
    
);
drop table HomeChef;
select * from HomeChef;
truncate table HomeChef;

CREATE TABLE RecipeWriter (
  RecipeWriterId int NOT NULL IDENTITY(4001,1) PRIMARY KEY, 
  UserId int FOREIGN KEY REFERENCES [User](UserId) NOT NULL,
  About       varchar(500), 
  FbLink      varchar(500), 
  InstaLink   varchar(500), 
  TwitterLink varchar(500) 
  
  );
  select * from RecipeWriter;
  truncate table RecipeWriter;

  CREATE TABLE PremiumUser (
	  PremiumUserId int NOT NULL IDENTITY(5001,1) PRIMARY KEY, 
	  UserId int FOREIGN KEY REFERENCES [User](UserId) NOT NULL,
	  PaymentStatus varchar(50)

  );
  select * from PremiumUser;
  truncate table PremiumUser;

  CREATE TABLE Blog (
  BlogId int NOT NULL IDENTITY(6001,1) PRIMARY KEY, 
  BloggerId int FOREIGN KEY REFERENCES Blogger (BloggerId) NOT NULL,
  BlogName varchar(255) NOT NULL, 
  BlogPic varchar(500) NOT NULL, 
  BlogPublishDate date NOT NULL, 
  BlogSummary     varchar(500) NOT NULL, 
  BlogDescription varchar(8000) NOT NULL, 

);
select * from Blog;
truncate table Blog;

CREATE TABLE BlogComment (
  BlogCommentId int NOT NULL IDENTITY(7001,1) PRIMARY KEY,
  BlogId int FOREIGN KEY REFERENCES Blog (BlogId) NOT NULL,
  UserId int FOREIGN KEY REFERENCES [User](UserId) NOT NULL,
  BlogComment  varchar(500) NOT NULL

);
select * from BlogComment;
truncate table BlogComment;

CREATE TABLE Contact (
  ContactId int NOT NULL IDENTITY(8001,1) PRIMARY KEY, 
  Email varchar(255) Not null,
  Phone varchar(11) not null,
  ContactMessage varchar(8000) NOT NULL, 
  UserId int FOREIGN KEY REFERENCES [User](UserId) NOT NULL
);
select * from Contact;
drop table Contact;
truncate table Contact;

CREATE TABLE HomeChefRating (
  HomeChefRatingId int NOT NULL IDENTITY(9001,1) PRIMARY KEY, 
  HomeChefRating   int NOT NULL, 
  HomeChefId   int FOREIGN KEY REFERENCES HomeChef(HomeChefId) NOT NULL, 
  UserId int FOREIGN KEY REFERENCES [User](UserId) NOT NULL 
  
);
select * from HomeChefRating;
drop table HomeChefRating ;

 CREATE TABLE HomeMadeFood(
   HomeFoodId int NOT NULL IDENTITY(10001,1) PRIMARY KEY,
   HomeChefId int NOT NULL FOREIGN KEY REFERENCES HomeChef(HomeChefId),
   HomemadeFoodName   varchar(255) NOT NULL,
   HomemadeFoodPrice  float NOT NULL,
   HomemadeFoodPic	varchar(500) ,
   AvailabilityStatus varchar(3) NOT NULL,
   date date  NULL
);

ALTER TABLE HomeMadeFood drop column date
alter table HomeMadeFood add date date NULL
select * from HomeMadeFood;
drop table HomeMadeFood ;

CREATE TABLE Recipe (
  RecipeId          int NOT NULL IDENTITY(12001,1) PRIMARY KEY,
  RecipeWriterId int FOREIGN KEY REFERENCES RecipeWriter(RecipeWriterId),
  RecipeName         varchar(255) NOT NULL, 
  RecipePublishDate  date NOT NULL, 
  RecipePicture      varchar(500) NOT NULL, 
  RecipeCookingTime  varchar(50) NOT NULL,
  RecipeServing      varchar(50) NOT NULL,
  RecipeIngredients  varchar(500) NOT NULL, 
  Process            varchar(8000) NOT NULL, 
  Description         varchar(500) NOT NULL, 
 
);

select * from Recipe;
drop table Recipe;
truncate table Recipe;
select * from Recipe where RecipeIngredients LIKE '%Chicken%' AND RecipeIngredients LIKE '%salt%';
CREATE TABLE RecipeRating (
  RecipeRatingId int NOT NULL IDENTITY(13001,1) PRIMARY KEY,
  RecipeRating   int NOT NULL, 
  RecipeId       int FOREIGN KEY REFERENCES  Recipe(RecipeId) NOT NULL,  
  UserId int FOREIGN KEY REFERENCES [User](UserId)
);

select * from RecipeRating;
drop table RecipeRating;

CREATE TABLE RecipeComment (
  RecipeCommentId int NOT NULL IDENTITY(14001,1) PRIMARY KEY, 
  RecipeId        int FOREIGN KEY REFERENCES  Recipe(RecipeId) NOT NULL, 
  RecipeComment   varchar(500) NOT NULL, 
  UserId int FOREIGN KEY REFERENCES [User](UserId), 
);
select * from RecipeComment;
drop table RecipeComment;

 CREATE TABLE Restaurant (
  RestaurantId   int NOT NULL IDENTITY(15001,1) PRIMARY KEY,
  RestaurantName varchar(255) NOT NULL,
  Location   	varchar(255) NOT NULL,
  Area varchar(50) NOT NULL,
  Offers     	varchar(6000),
  ProfilePic 	varchar(500) ,
  CoverPic  	varchar(500),
  RestaurantPassword varchar(50)
  );
  
  select * from Restaurant;
  drop table Restaurant;
  truncate table Restaurant;

  CREATE TABLE Menu (
  MenuId int NOT NULL IDENTITY(11001,1) PRIMARY KEY,
  FoodName  varchar(255) NOT NULL, 
  FoodIngredient  varchar(8000) NOT NULL, 
  Price  float NOT NULL, 
  RestaurantId int FOREIGN KEY REFERENCES Restaurant(RestaurantId) NOT NULL, 
  FoodPandaOffers varchar(255),
  HungriNakiOffers varchar(255),
  UberEatsOffers varchar(255)
);
select * from Menu;
select * from Restaurant;
SELECT RestaurantName FROM Restaurant where RestaurantPassword = 'ZmF0Ym95';
drop table Menu;
truncate table Menu;

CREATE TABLE RestaurantRating 
(
  RestaurantRatingId int NOT NULL IDENTITY(16001,1) PRIMARY KEY,
  Rating int NOT NULL, 
  RestaurantId int FOREIGN KEY REFERENCES Restaurant(RestaurantId),
  UserId int FOREIGN KEY REFERENCES [User](UserId)
  
);

select * from RestaurantRating;
drop table RestaurantRating;
truncate table RestaurantRating;
select Restaurant.RestaurantId, Restaurant.RestaurantName,Restaurant.Location, Restaurant.Area,Restaurant.Offers,Restaurant.ProfilePic,Restaurant.CoverPic,Restaurant.RestaurantPassword,avg(RestaurantRating.Rating) from Restaurant left join RestaurantRating on RestaurantRating.RestaurantId = Restaurant.RestaurantId group by Restaurant.RestaurantId, Restaurant.RestaurantName,Restaurant.Location, Restaurant.Area,Restaurant.Offers,Restaurant.ProfilePic,Restaurant.CoverPic,Restaurant.RestaurantPassword,RestaurantRating.RestaurantRatingId,RestaurantRating.Rating,RestaurantRating.RestaurantId,RestaurantRating.UserId;
select Restaurant.RestaurantId, Restaurant.RestaurantName,Restaurant.Location, Restaurant.Area,Restaurant.Offers,Restaurant.ProfilePic,Restaurant.CoverPic,Restaurant.RestaurantPassword,subquery.avgRate 
from Restaurant,(Select RestaurantRating.RestaurantId,avg(RestaurantRating.Rating) as avgRate 
from RestaurantRating group by RestaurantRating.RestaurantId) subquery
where subquery.RestaurantId = Restaurant.RestaurantId;

Select RestaurantRating.RestaurantId,avg(RestaurantRating.Rating) as avgRate 
from RestaurantRating 
group by RestaurantRating.RestaurantId
having RestaurantRating.RestaurantId = RestaurantId;


 CREATE TABLE User_Recipe(
  UserId int FOREIGN KEY REFERENCES [User](UserId) NOT NULL ,
  RecipeId int FOREIGN KEY REFERENCES Recipe(RecipeId) NOT NULL, 

  CONSTRAINT PK_User_Recipe PRIMARY KEY (UserId, RecipeId)
)
select * from User_Recipe;
drop table User_Recipe;
truncate table User_Recipe;

CREATE TABLE User_HomeMadeFood(
  UserId   int NOT NULL FOREIGN KEY REFERENCES [User] (UserId),
  HomeFoodId int NOT NULL FOREIGN KEY REFERENCES HomeMadeFood(HomeFoodId)
  CONSTRAINT PK_User_HomeMadeFood PRIMARY KEY (UserId, HomeFoodId)

);

select * from User_HomeMadeFood;
drop table User_HomeMadeFood;

CREATE TABLE User_Blog (
  UserId int FOREIGN KEY REFERENCES [User](UserId),
  BlogId int FOREIGN KEY REFERENCES Blog (BlogId),
  CONSTRAINT PK_User_Blog PRIMARY KEY (UserId, BlogId)
);

select * from User_Blog ;


CREATE TABLE User_Restaurant (
    UserId int FOREIGN KEY REFERENCES [User](UserId),
    RestaurantId int FOREIGN KEY REFERENCES Restaurant(RestaurantId),
	CONSTRAINT PK_User_Restaurant PRIMARY KEY (UserId, RestaurantId)
);

select * from User_Restaurant ;
drop table User_Restaurant;
truncate table User_Restaurant;

CREATE TABLE Reservation (
	ReservationId int NOT NULL IDENTITY(17001,1) PRIMARY KEY,
	UserId int FOREIGN KEY REFERENCES [User](UserId),
    RestaurantId int FOREIGN KEY REFERENCES Restaurant(RestaurantId),
);
select * from Reservation;
drop table Reservation;

create table User_access
(
	useraccessId int identity(21001,1) primary key,
	UserId int FOREIGN KEY REFERENCES [User](UserId),
	homechefaccess int ,
	bloggeraccess int,
	recipewriteraccess int
);
select * from User_access;
drop table User_access;
truncate table User_access;

create table community 
(
	CommunityId int identity(21001,1) primary key,
	UserId int FOREIGN KEY REFERENCES [User](UserId),
	Comment varchar(255) NOT NULL

);
select * from community;
select * from Menu;
select * from restaurant inner join menu on menu.RestaurantId = restaurant.RestaurantId;
select * from community inner join [User] on community.UserId =[User].UserId;
select* from Blog left join Blogger on Blog.BloggerId = Blogger.BloggerId left join[User] on Blogger.UserId =[User].UserId left join BlogComment on Blog.BlogId = BlogComment.BlogId left join[User] as userComment on BlogComment.UserId = userComment.UserId right join community on community.userId = userComment.UserId ;
select * from Blog inner join Blogger on Blog.BloggerId=Blogger.BloggerId inner join [User] on Blogger.UserId=[User].UserId;

select * from Blog inner join Blogger on Blog.BloggerId=Blogger.BloggerId inner join [User] on Blogger.UserId=[User].UserId inner join community on community.userId = [User].UserId 
select * from blogger;
select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId;
alter table Recipe add  RecipeServing  varchar(50) NOT NULL
select * from Contact;
sp_rename 'Recipe.RecipeWriterUserId', 'RecipeWriterId', 'COLUMN';
sp_rename 'Recipe.RecipeWriterUserId', 'RecipeWriterId', 'COLUMN';
sp_rename 'HomeMadeFood.HomeChefUserId', 'HomeChefId', 'COLUMN';


alter table Restaurant add RestaurantPassword varchar(50);
select * from [User] Inner join HomeChef on [User].UserId = HomeChef.UserId where HomeChef.UserId = 1026;
select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefUserId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId;
insert into HomeMadeFood values(3003,'MuttonRezala',250.00,'D:\Labonno study\3.2\SD LAB\Doc&Dine copy27\DocAndDine\DocAndDine\Content\Images\MuttonRezala1.jpg','YES');
alter table HomeMadeFood add date varchar(50) ;
select * from Menu where RestaurantId = '15001';

select * from Recipe inner join RecipeWriter on Recipe.RecipeWriterId = RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId where RecipeId = '12001';
select * from Recipe inner join RecipeWriter on Recipe.RecipeWriterId=RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId;
select * from Recipe inner join RecipeWriter on Recipe.RecipeWriterId = RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId left join RecipeComment on Recipe.RecipeId = RecipeComment.RecipeId left join [User] as userComment on RecipeComment.UserId = userComment.UserId;
select * from BlogComment;
select * from Blog inner join Blogger on Blog.BloggerId=Blogger.BloggerId inner join [User] on Blogger.UserId=[User].UserId left join BlogComment on Blog.BlogId= BlogComment.BlogId left join [User] as userComment on BlogComment.UserId = userComment.UserId where Blog.BlogId=6003;

select Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink,[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from Recipe inner join RecipeWriter on Recipe.RecipeWriterId=RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink;




insert into [user] values
		('Marufa Kamal','marufakamal.labonno@gmail.com','MQ==','01682278884','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\recipeProfile2.png'),
		('Anika Tanzim','anikatanzim@gmail.com','MTIzNA==','01758900632','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\recipeProfile2.png'),
		('Abanti Chakraborty','abantichakraborty@gmail.com','MTIzNDU=','01788163241','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\recipeProfile2.png'),
		('Ashraf Ayon','ashrafayon@gmail.com','MTIz','01682278884','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Rashedul Raj','rashedulraj@gmail.com','MQ==','01672278452','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Arif Ishan','arifishan@gmail.com','MTIzNA==','01751963241','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Zahid Hasan','zahidha@gmail.com','MTIzNDU=','01758965541','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Nishat Tahira','nishat@gmail.com','MTIz','01788963266','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Arunima Ayshee','arunima@gmail.com','MQ==','01712278452','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Aniqua','aniqua@gmail.com','MTIzNA==','01634278451','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Rahat Osman','rahatn@gmail.com','MTIzNDU=','01682278752','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Arafat','arafat@gmail.com','MTIz','01682278452','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('robiul Islam','robiul@gmail.com','MQ==','01682273452','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Mir Rahat','mirrahat@gmail.com','MTIzNA==','01682245452','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png'),
		('Jabir','jabir@gmail.com','MTIz','01682278423','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\admin.png');


	
insert into homeChef values
(1003,'https://www.facebook.com/','https://www.instagram.com/labonno101/?hl=en','https://twitter.com/?marufa','Jahanara garden','Green road'),
(1004,'https://www.facebook.com/','https://www.instagram.com/anikatanzim1/?hl=en','https://twitter.com/?anika','sideshwari','Mokhbazar'),
(1005,'https://www.facebook.com/','https://www.instagram.com/abanti12/?hl=en','https://twitter.com/?shruti','atish dipankar road','Shabujbagh');



insert into RecipeWriter values
(1006,'I love to cook and share my recipes','https://www.facebook.com/ayon','https://www.instagram.com/?ayon','https://twitter.com/?ayon'),
(1007,'Food is love','https://www.facebook.com/raj','https://www.instagram.com/?raj','https://twitter.com/?raj'),
(1008,'Cook your homemadefood and stay healthy','https://www.facebook.com/ishaan','https://www.instagram.com/?ishaan','https://twitter.com/?ishaan');

insert into Blogger values
(1015,'64/A Sideswarri road, Ramna, Dhaka-1217','I’m a food lover, number cruncher, and meticulous budgeter. I love science and art, and the way they come together when I cook. I love to create, problem solve, and learn new things. I’m not a professional chef or financial expert, but I love learning by doing and sharing my experiences. ','2','Bsc passed in CSE from AUST','25','Savory'),
(1016,'Sector-11 ,road-14, house-7, Uttara, Dhaka-1230','The main reason why I became inspired to start food-blogging in the first place is because I have always been passionate about food and food photography. Everything about food excites me – the taste, the flavors, the presentation, everything. Discovering new restaurants, dishes and cuisines is what drives me.','5','Msc in Physics from Dhaka University','36','Sweet'),
(1017,'1/A New Baily Road, Dhaka-1000','I started my journey as a food-blogger when I realized that people in Dhaka were being deprived of honest and unbiased reviews. Since starting my Facebook page, I have received unbelievable support from people around me and is astonished at how other foodies such as myself value my opinion and make decisions based on my opinion.','10','MA in English from SUST','43','Savory');


insert into HomeMadeFood values
(3001,'Mutton polao','250.0','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\MuttonRezala1.jpg','YES','2019-05-29'),
(3002,'Sandwich','170.0','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\sandwich1.jpg','YES','2019-07-17'),
(3003,'Gulab Jamun','200.0','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\almond-flour-gulab-jamun.jpg','YES','2019-08-19');

insert into Recipe values
('4001','Chicken Kabab','2019-05-29','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\images.png','2hr','6','Chicken,curd,oil,salt,masala','Mix all the ingredients.Marinate them and put it in oven','Easy kabab recipe'),
('4003','Pancake','2019-07-15','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\pancake.png','15min','4','3cup Flour,suger,baking powder,salt,milk','Mix all the ingredients.Add milk.Mix until smooth.Pour on a frying pan & brown sides.','Instant pancakes'),
('4001','Waffles','2019-09-22','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\waffles.png','20min','6','2cup Flour,2eggs,1cup oil,Milk,suger,vanila','Preheat waffles iron.Make the batter with ingredients.Spray it & Cook until golden brown','Yummy waffles!');

insert into Blog values
('2001','The Margherita pizza','~/Content/Images/The Margherita pizza.png','2019-03-03','The Margherita pizza had both mozzarella and parmesan used in it giving it a very cheesy and nutty taste. The tomato base infused with olive oil went well with the cheese overall giving me a tangy taste. The crust was thick and yet the slight burns gave it a rustic taste. ','The crust was thick and yet the slight burns gave it a rustic taste. They had served chili flakes, chili and a garlic mustard sauce alongside the pizza and surprisingly, it was the kick of the mustard sauce that I found the best to go with the pizza.As for the Grilled Chicken, it came with sautéed vegetables and a mashed potato. What brought a smile on my face was the vibrant color of the plate and the presentation. If more restaurant owners would step up their presentation game, customers would happily pay more for the dish.'),
('2002','Korean Charbroiled Grilled Meal','~/Content/Images/KoreanCharbroiledGrilledMeal.png','2018-02-04','This meal was an absolute steal for the price! Other than the rice being clumpy from being pre-prepared since the offer was selling like hotcakes, I thoroughly enjoyed the diverse flavor (spicy,smoky, and sweet) coming from the wings, charboiled chicken, and the fried chicken.','The meal Included: Korean Charbroiled Grilled (Whole Leg), Garlic Fried Rice, Gangnam Fried Chicken, BB Chicken Wings, Fresh Salad, Blue Curaçao Fusion Drinks.This meal was an absolute steal for the price! Other than the rice being clumpy from being pre-prepared since the offer was selling like hotcakes, I thoroughly enjoyed the diverse flavor (spicy,smoky, and sweet) coming from the wings, charboiled chicken, and the fried chicken.While, the inclusion of a drink with the meal really got our group excited, the syrup had not been mixed at all with the soda water even after stirring it multiple times. Makes me wonder if it was syrup or Saccharin (an artificial, non-nutritive sweetner which is also considered harmful for the body).'),
('2003','Simple Sweet and Sour Sauce','~/Content/Images/Simple-Sweet-and-Sour-Sauce.jpg','2018-10-03','One of my biggest pet peeves is having a million half-used condiments in my fridge. So if I can quickly whip up a homemade sauce with pantry staples, I’m going to do that before buying another bottle! This Simple Sweet and Sour Sauce is the perfect example. At its most basic, a sweet and sour sauce is just sugar and vinegar, flavored with a splash of soy sauce and ketchup. But there are also plenty of ways you can jazz it up and make it your own, so I’ll also include those below!','I like to make my homemade sauces in small batches so there are no leftovers because having a million half-used homemade sauces in your fridge is just as annoying as having a million half-used bottled sauces in your fridge. The recipe below makes about 1/2 cup of sauce, but can easily be doubled or tripled if needed. Simply adjust the number of servings in the servings box in the recipe card and all of the ingredient quantities will adjust for you. There will be no change to the cooking method. ');

insert into Restaurant values
('Chillox','Level3,AMM Center,56/A Rd 3A,Dhaka','Premium burgers@160tk','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\chillox.jpg','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\ChilloxMenu.jpg','Dhanmondi','MQ=='),
('Dominos',' Rangs Fortune Square, Gnd & 1st Floor,Plot no #32, Rd 02,Dhaka','Dominos day@149tk','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\dominos.jpg','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\ChilloxMenu.jpg','Dhanmondi','MTIzNA=='),
('Sultans Dine','Samsuddin Mansion(1st floor), House: 41,Road: 52, Dhaka','','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\suultans.jpg','E:\SHRUTI\3.2\CSE3200 - SD Lab\PROJECT\Doc&Dine copy34\DocAndDine\DocAndDine\Content\Images\ChilloxMenu.jpg','Gulshan','MQ==');

insert into Contact values
('ashrafayon@gmail.com','01682278884','Im facing problems in posting recipe','1006'),
('abantichakraborty@gmail.com','01788163241','Home made food page is loading slowly','1005'),
('aniqua@gmail.com','01634278452','Facing problems in login','1012');

insert into Menu values
('Chicken cheese burger','Chicken patty,cheese,sauce',128.0,'15001'),
('Binge chicken burger','Chicken patty,smoked chicken,chicken ham,cheese',496.0,'15001'),
('Deluxe feast',' Spicy Chicken, Grilled Chicken Rashers,Capsicum,Mushrooms',149.00,'15002'),
('Kacchi','Kacchi,polao,borhani',486,'15003'),
('Kacchi platter with jorda','Kacchi,roast,kabab,rezala,borhani',566.00,'15003');

insert into PremiumUser values
('1009','Paid'),
('1010','Not Paid'),
('1011','Paid');

insert into HomeChefRating values
('3','3001','1011'),
('4','3003','1015'),
('5','3002','1017');


insert into BlogComment values
('6001','1010','This pizza looks prettty damn good'),
('6002','1011','I love bbq bd but tend to avoid during holidays'),
('6003','1012','Wow so much yummy!');

insert into RecipeRating values
('4','12002','1009'),
('5','12004','1010'),
('4','12003','1013');

insert into RecipeComment values
('12002','Thats helpful','1013'),
('12003','Came out yummy','1015'),
('12002','Thanks','1011');

insert into Reservation values
('1017','15001'),
('1008','15002'),
('1015','15003');

insert into RestaurantRating values
('4','15001','1016'),
('5','15002','1012'),
('4','15003','1013');

insert into User_Recipe values
('1059','12012'),
('1059','12010'),
('1060','12011');

insert into User_Blog values
('1063','6001'),
('1067','6002'),
('1068','6003');

insert into User_HomeMadeFood values
('1061','10011'),
('1060','10013'),
('1058','10012');

insert into User_Restaurant values
('1062','15001'),
('1063','15002'),
('1070','15003');

insert into user_access values
(1003,1,0,0),
(1004,1,0,0),
(1005,1,0,0),
(1006,0,0,1),
(1007,0,0,1),
(1008,0,0,1),
(1015,0,1,0),
(1016,0,1,0),
(1017,0,1,0);