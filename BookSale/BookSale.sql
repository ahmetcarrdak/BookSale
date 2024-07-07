create table Sale
(
    Id          int auto_increment
        primary key,
    Name        varchar(255)  null,
    Phone       varchar(20)   null,
    Address     varchar(255)  null,
    Mail        varchar(255)  null,
    IsMember    tinyint(1)    null,
    book_id     int           null,
    status      int default 0 null,
    customer_id int           null,
    Piece       int           null,
    member_id   int default 0 null
);

create table admins
(
    id       bigint unsigned auto_increment
        primary key,
    name     varchar(255) not null,
    mail     varchar(255) not null,
    password varchar(255) not null,
    constraint id
        unique (id),
    constraint mail
        unique (mail)
);

create table books
(
    id          bigint unsigned auto_increment
        primary key,
    customer_id int            null,
    image       text           null,
    title       varchar(255)   not null,
    subject     varchar(255)   null,
    type        varchar(255)   null,
    description text           null,
    price       decimal(10, 2) not null,
    stock       int            not null,
    author      varchar(255)   null,
    constraint id
        unique (id)
);

create table cart
(
    id          bigint unsigned auto_increment
        primary key,
    member_id   int           null,
    customer_id int           null,
    books_id    int           null,
    piece       int default 1 not null,
    constraint id
        unique (id)
);

create table contact
(
    id          bigint unsigned auto_increment
        primary key,
    name        varchar(255) not null,
    mail        varchar(255) not null,
    subject     varchar(255) null,
    description text         null,
    constraint id
        unique (id)
);

create table coupon
(
    id          int auto_increment
        primary key,
    coupon_id   text null,
    customer_id int  not null,
    member_id   int  null,
    ValueCoupon int  null
);

create table customers
(
    id       bigint unsigned auto_increment
        primary key,
    name     varchar(255)  not null,
    phone    varchar(15)   null,
    mail     varchar(255)  not null,
    password varchar(255)  not null,
    status   int default 0 null,
    constraint id
        unique (id)
);

create table member
(
    id       bigint unsigned auto_increment
        primary key,
    name     varchar(255) not null,
    phone    varchar(15)  null,
    mail     varchar(255) not null,
    address  text         null,
    password varchar(255) not null,
    constraint id
        unique (id)
);

create table pastCarts
(
    id          int auto_increment
        primary key,
    member_id   int not null,
    customer_id int not null,
    book_id     int not null,
    piece       int not null
);

