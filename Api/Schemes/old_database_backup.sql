-- phpMyAdmin SQL Dump
-- version 5.0.4deb2+deb11u1
-- https://www.phpmyadmin.net/
--
-- Počítač: localhost:3306
-- Vytvořeno: Úte 25. říj 2022, 06:28
-- Verze serveru: 10.5.15-MariaDB-0+deb11u1
-- Verze PHP: 7.4.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Databáze: `cck`
--
CREATE DATABASE IF NOT EXISTS `cck` DEFAULT CHARACTER SET utf8 COLLATE utf8_czech_ci;
USE `cck`;

-- --------------------------------------------------------

--
-- Struktura tabulky `competitions`
--

CREATE TABLE `competitions` (
  `id` bigint(20) NOT NULL,
  `startDate` datetime NOT NULL,
  `endDate` datetime NOT NULL,
  `type` enum('okresní','krajské','republikové','mezinárodní','testovací') COLLATE utf8_czech_ci NOT NULL,
  `description` text COLLATE utf8_czech_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

--
-- Vypisuji data pro tabulku `competitions`
--

INSERT INTO `competitions` (`id`, `startDate`, `endDate`, `type`, `description`) VALUES
(1, '2022-08-26 20:44:18', '2022-08-30 20:44:18', 'okresní', 'Testovácí kolo závodu ČČK');

-- --------------------------------------------------------

--
-- Struktura tabulky `figurants`
--

CREATE TABLE `figurants` (
  `id` bigint(20) NOT NULL,
  `injurie_id` bigint(20) NOT NULL,
  `instructions` text COLLATE utf8_czech_ci NOT NULL,
  `makeup` text COLLATE utf8_czech_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Struktura tabulky `injuries`
--

CREATE TABLE `injuries` (
  `id` bigint(20) NOT NULL,
  `station_id` bigint(20) NOT NULL,
  `referee_id` bigint(20) NOT NULL,
  `letter` char(1) COLLATE utf8_czech_ci NOT NULL,
  `situation` text COLLATE utf8_czech_ci NOT NULL,
  `diagnosis` text COLLATE utf8_czech_ci NOT NULL,
  `maximalPoints` int(11) NOT NULL,
  `necessaryThings` text COLLATE utf8_czech_ci DEFAULT NULL,
  `info` text COLLATE utf8_czech_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Struktura tabulky `result_injuries`
--

CREATE TABLE `result_injuries` (
  `id` bigint(20) NOT NULL,
  `team_id` bigint(20) NOT NULL,
  `injurie_id` bigint(20) NOT NULL,
  `referee_id` bigint(20) NOT NULL,
  `referee_signature` varchar(512) COLLATE utf8_czech_ci NOT NULL,
  `leader_signature` varchar(512) COLLATE utf8_czech_ci NOT NULL,
  `signed` datetime NOT NULL,
  `created` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Struktura tabulky `result_tasks`
--

CREATE TABLE `result_tasks` (
  `id` bigint(20) NOT NULL,
  `result_injurie_id` bigint(20) NOT NULL,
  `task_id` bigint(20) NOT NULL,
  `deducted_points` int(11) NOT NULL,
  `created` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Struktura tabulky `stations`
--

CREATE TABLE `stations` (
  `id` bigint(20) NOT NULL,
  `competetion_id` bigint(20) NOT NULL,
  `title` text COLLATE utf8_czech_ci NOT NULL,
  `type` enum('volný','standdart','improvizace') COLLATE utf8_czech_ci NOT NULL,
  `tier` enum('unknown','první stupěň','druhý stupeň') COLLATE utf8_czech_ci NOT NULL,
  `number` int(11) NOT NULL,
  `created` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

--
-- Vypisuji data pro tabulku `stations`
--

INSERT INTO `stations` (`id`, `competetion_id`, `title`, `type`, `tier`, `number`, `created`) VALUES
(1, 1, 'Uzavřená zlomenina', 'standdart', 'první stupěň', 1, '2022-09-11 02:41:30'),
(2, 1, 'Otevřená zlomenina', 'standdart', 'první stupěň', 1, '2022-09-11 02:42:38'),
(4, 1, 'Otevřená zlomenina', 'standdart', 'první stupěň', 1, '2022-09-11 02:42:39'),
(5, 1, 'Otevřená zlomenina', 'standdart', 'první stupěň', 1, '2022-09-11 02:42:40');

-- --------------------------------------------------------

--
-- Struktura tabulky `tasks`
--

CREATE TABLE `tasks` (
  `id` bigint(20) NOT NULL,
  `injurie_id` bigint(20) NOT NULL,
  `title` varchar(64) COLLATE utf8_czech_ci NOT NULL,
  `maximalMinusPoints` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Struktura tabulky `teams`
--

CREATE TABLE `teams` (
  `id` bigint(20) NOT NULL,
  `title` varchar(64) COLLATE utf8_czech_ci NOT NULL,
  `organization` text COLLATE utf8_czech_ci DEFAULT NULL,
  `leader_id` bigint(20) NOT NULL,
  `escort_id` bigint(20) DEFAULT NULL,
  `memberCount` int(11) NOT NULL,
  `points` int(11) DEFAULT NULL,
  `competetion_id` bigint(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Struktura tabulky `threathment_procedures`
--

CREATE TABLE `threathment_procedures` (
  `id` bigint(20) NOT NULL,
  `injurie_id` bigint(20) NOT NULL,
  `activity` varchar(128) COLLATE utf8_czech_ci NOT NULL,
  `order` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- --------------------------------------------------------

--
-- Struktura tabulky `tokens`
--

CREATE TABLE `tokens` (
  `id` varchar(36) COLLATE utf8_czech_ci NOT NULL,
  `user_id` bigint(20) NOT NULL,
  `token` varchar(512) COLLATE utf8_czech_ci NOT NULL,
  `created` datetime NOT NULL DEFAULT current_timestamp(),
  `expiration` int(11) NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

--
-- Vypisuji data pro tabulku `tokens`
--

INSERT INTO `tokens` (`id`, `user_id`, `token`, `created`, `expiration`) VALUES
('1edfc715-8bbc-4a73-8dd4-b788da9c236b', 1, 'N4IhFeBO3lIA57r3mWnhMCuReO8N5ND65dHCZbQpo1dGr1ovD5SzwrUYm8PTV447bTni2a8WwYWJDFnOBzTzKVTxFx6BrubAwdebvAnKQ9IurRwcqC46rgdVwYsHqxJT', '2022-09-25 21:40:41', 86400),
('31e34668-92c0-4b8b-bc58-9adebd12804e', 1, 'yb6vksfDrf4SIqdulshMpn6ZimkWmgCN5cPzFAphRFNrd4J2WgphRAkqI4aeoypeo0rRJl22GZSKTL2a1EsFStaGx1nZCsIPYzPalB3BOZBsXgUqeVBfuVB3ILDDFxmM', '2022-09-25 21:40:42', 86400),
('4b3a8aa9-822c-4467-9567-787aff876631', 1, 'n43ZX7VklvcHKPT1RPuLGvmJQBP4xbn31V59R9BZnWYm7GY9GhZu32NMUCQkzoPstGmlk1b8xaFuI7XMFXLa0QRwL78jahlpUSfK8MFp9OVD7KIEd4ZQxR7VjCpSq1ux', '2022-09-10 16:59:45', 86400),
('4ccde8bb-9d1d-48c4-a9cd-bad0e56494ad', 2, 'QwWehD6e2DAgbtvteI97sKBE1Kj4LdPlPbKikrUdmFQRhkQhDI120XSXFf7npgwtjhTFfZNs7pYCeCwOV00KG5JSOWuxG620FqhKw58K1eibdlFVnafqRAPdhROmQ7KA', '2022-09-01 15:21:12', 86400),
('5aa2959f-5ce6-4fd4-956a-5593f9278b7f', 2, 'hGjIlKaaxqmm0OiIjne3ZeXDXI02lBE0EFtEDg6s1Wyziln3cOeuSDMKo3LL3Kf6PAeoKY96HHYhZSfy6A2mwDikkPNbqoCHAn55T4d0lB5uBF9OhXZtBKltCs9rM3gP', '2022-09-01 15:21:27', 86400),
('5e21ae60-a5b1-4d04-9f03-6a7747852a6b', 2, 'IrGl0pTHow3ecCIe4Nay9sdN6g4y468h1P48Flj9DDAPeYbrKzrfB77Llt1uf6IcuM9RLvwkNdqp02W6R9a3bZk54JiWGTnURDDCjBIDETCH8JbBj0trDhD1hQ1rZHsW', '2022-09-22 10:09:55', 86400),
('68ee8c64-7706-4a4c-b9a0-eec8600b67b6', 2, 'YGkdJ63JNfEdstBHVYneAP2OvErWGLmuoqeHT30HeKeCzJdMTzRbnc0DxfCKZDCZshHDbgB3bW98TQ8sj8tqY24QVLyJ76QCKjqGPXLYaVMLKvwCmI5AuckkVy4JBmHY', '2022-09-01 15:19:48', 86400),
('835fae71-29dd-4471-a02c-78f02439be5e', 1, 'ocwiTlp7SAvGSSkew9758BVzBpX6oFbAv8oTOQS2oQL7KmhrRppChJCo7mll31LVBDpZGpPhHNa2lgzHEG8vQtw5aNOHUX0I2UUp14vmQ4DwKEp53WPFk65LGtioPcuO', '2022-09-25 21:40:40', 86400),
('8825fbdf-bb39-45a6-b395-f41f8c1a8f88', 1, 'lEZDUVYbL8drVzuoTWANTrB6EynSx1XKCp7RzVJMFrNDz7UDMADwuvBX7JgQFmACDvUmEFqCCF5gLIAHUT6LYsY8IrxWMRB3mlRXoACp4eiDxfTdenvIDKJHDGL1jkUf', '2022-09-10 17:10:54', 86400),
('beaa622d-fd1d-4505-9b50-5f68f6b2fe58', 2, '2w3TSglsof8zkpTlQrpz21AxoijMfMa8HKTdQzyHphLb2nyXZmHc0uDnWr5HzeAqxSn4NuKo0vdbjuDmCWj2WO272R8Us2Mky3VwTO3yXmCyz94cka6jRP9k533leTep', '2022-09-01 15:16:40', 86400),
('c4664116-9b71-4041-8e82-ed703967d20d', 1, 'DaR9qdxDHXGgeM4r8uyJfZ4gA4Y2rnIzjfUhGR2dH6LQcVyLGQitN189dym08fa9yWSs1cI1mGuU3ipgn6AAFmcEKxQkm3Gkkk40gomAG3Yz1Y8MiGzBXS27QYCGAHIX', '2022-09-25 21:40:20', 86400),
('c555dad5-dc16-44a1-8e16-b18ee22f4ee9', 1, 'jS0QO63TcFTJ6XRz2390a5eiw4XpbZKoJrgklW8y4Pn3I8WTtpbrJIhpOUlCMRSIOBipgLePsDDLlO4cqmaJa1pmfjDWmFEi72egN7LecZtP7yDOHwDwiW5Cn6Hfcvtg', '2022-09-11 16:07:30', 86400),
('d39e3a72-d74b-47ed-8b36-90d88824f4cb', 2, 'djdek0nLZlza7OrcwVdbG2Ew65f1vSIn3EmuTl8VcUwho0CsZAwI7dIM1xxlaUO2tdVuVf6Ih9vtxFOV3mefKJmRHeh81z5nbEHTlAXVYkHYbRwEY7oJSDoHWdjvUtsl', '2022-09-01 15:02:48', 86400),
('e2293af7-48f8-49f5-941d-9ff7e194b96d', 2, 'ao16zqUM9GbilS82U3KlJ1bD0rbSM9qF0tUhJybtBQkYDyviIcd9MeWtoI4b2K6RNN4bExJFrMhg6n9Y8XShmBbUax6t1GopqZLVUps9zXgFzt8JXmVTLAk3dI5g45w0', '2022-09-01 15:25:43', 86400),
('f20a4f19-38e3-48e5-9a16-2feb9aa8e28f', 2, 'rinzKsQR8YOT2SaTtgBTPSe0ChBKJqYcG3jREOyHSQzVfYbwIUmmUJ1lBeg5ODNM71J62nbQHzkuux8vCA9H3xIlKzCKKI68ikZikN0zYaEWJ6eJ0AIOCijKYvXFR9Hj', '2022-09-01 15:17:59', 86400),
('f8953211-fc6b-4674-863e-c97fe2d1f185', 1, 'GelpS0m1LiPDMiyTMFnEXeKbfgNSbtLBl4BaxMQyielRuW0nyOg92osSGNmvbmhqHU8NSmc5ltTnrMxIK4XS1zmEcSv9JMCBXi5XWOX9jnIWmsC0LnxJhfAH5vizf9ii', '2022-09-01 14:59:11', 86400),
('f9680c94-f17c-488f-a801-e57e07d094f9', 2, 'jgeTpeZyZWsXKuOBj0qGAHs7LwGeygbv53y20avnmCS72tMHhEdWiXNxHaESU8Nc5wxa1fF5XtKwfGKmx01vSMzrJh3lMfkc9adId3OHzwUyy0XW06zchUS0mf2wiuVt', '2022-09-05 15:16:19', 86400);

-- --------------------------------------------------------

--
-- Struktura tabulky `users`
--

CREATE TABLE `users` (
  `id` bigint(20) NOT NULL,
  `user_template_id` bigint(20) NOT NULL,
  `competetion_id` bigint(20) NOT NULL,
  `signature` varchar(512) COLLATE utf8_czech_ci NOT NULL,
  `password` varchar(512) COLLATE utf8_czech_ci NOT NULL,
  `type` enum('rozhodčí','podpora','učitel','doprovod','velitel','soutěžící','host') COLLATE utf8_czech_ci NOT NULL,
  `active` tinyint(4) NOT NULL DEFAULT 1,
  `created` datetime NOT NULL DEFAULT current_timestamp(),
  `organization` varchar(128) COLLATE utf8_czech_ci DEFAULT NULL,
  `lastLogin` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

--
-- Vypisuji data pro tabulku `users`
--

INSERT INTO `users` (`id`, `user_template_id`, `competetion_id`, `signature`, `password`, `type`, `active`, `created`, `organization`, `lastLogin`) VALUES
(1, 1, 1, 'LeXeWJwRzH6bKVKOf1akBGKcuUSnPQLTusFo86cM1ZRl8ZJJNMOOsAonZte5DxjRJPk8VUz583WEtH58RsK9fadmxQkk3XcEwflrB3GZhvCyc57Qehk4aGOeJgdEeG2aNoJnepONVV3HwoQbW8DNhytdfTpQKUTRw3Q8duKytYVRsNYd1nepSyMF4zA5wD2GGkFvfErEQ98mQAtKbkhCpK2UJhibX5ST71UCmYVzPI1sskdV0CdHp3btdnijSWgwIY3QVhPlwxsueNBs2MivLnWnYUT67y29stxMazGVrIS3TVjgBaACNqa3iVj1emSGBwqK4jGjGW5MCgFjfzBQw5FPZyaONPOmj41DraMJo2z4z2VoYVOK2EQIIZ3uUnBrDlFSSK2ZRVMExjMopZUrE8zljVVdTs8ZZ5HNFSEZEnpcbdN3cw1aDQmGwu2vIyF1XGwRM61fmMGBkdtPED1z7yK9nvLrQbKzfba2AeI1nSVonAvzxaQRWDhq8CJERwSV', '$2a$11$WQMo.6w83XDyi1HZByjeoOMy8MJ0L1bsvJGoQN/TwK10AV5RpTxUS', '', 1, '2022-09-01 14:53:24', 'SPS-CL', '2022-09-25 21:40:42'),
(2, 2, 1, '4Q9yEOcguMOT5tDrlAeXuS5jCfGLSEoRnAWD5gCZJjo2ITy3rfcr6wu3SfZxpSdNGmPjiKlqlmMNdLiaZGxBzm0axOUpR5oUDMMrE7l5pevSIcs8kI27Lz0VWvuKWpyd5svcbf6IeFO9L66iGlkcZuMTf12sgI49JryGBNs2Ajd8X4HVnp7Gg6zdhfMWYV8lGS4sjGlyo0fu8kmmY0jV9OgKzkG8cjaRwviczu8f6EtO33AGkR0BPLfff9R5B7amtwPZ8M6c8QlEOO8xdRLTyJnbrIRrDKSnlf23OgvKIbBLSBmxbUzR0nrR27turKvltbNKQJ84phtFgaI11tbsEF0AOKUGVOkgsasTwVH3YJDTYE6NOGDhB6B1ryXwP0RQC6lF7OTJbaFYuWwJfRZma2Ml8EosIa1Du3saSzkGrp1Ke8AOsM5LBMBDnVqiPdijZV3ECZRhoHyovuorfauQJvoKptzYafnoeyDG4oQxgBiEB5LsOty6caXov081OIp4', '$2a$11$dolm02.64j4F45wKL4/WKu0Zn7pvP9xKJSVwj8lsIcAR44pEC8hT.', '', 1, '2022-09-01 14:54:22', 'SPS-CL', '2022-09-22 10:09:55');

-- --------------------------------------------------------

--
-- Struktura tabulky `user_templates`
--

CREATE TABLE `user_templates` (
  `id` bigint(20) NOT NULL,
  `firstName` varchar(45) COLLATE utf8_czech_ci NOT NULL,
  `lastName` varchar(45) COLLATE utf8_czech_ci NOT NULL,
  `email` varchar(45) COLLATE utf8_czech_ci NOT NULL,
  `phoneNumber` varchar(45) COLLATE utf8_czech_ci NOT NULL,
  `registered` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

--
-- Vypisuji data pro tabulku `user_templates`
--

INSERT INTO `user_templates` (`id`, `firstName`, `lastName`, `email`, `phoneNumber`, `registered`) VALUES
(1, 'Lukáš', 'Moravec', 'lukasl32@atlas.cz', '776300258', '2022-08-26 13:44:07'),
(2, 'David', 'Vobruba', 'david.vobruba@student.sps-cl.cz', '', '2022-09-01 07:08:03');

--
-- Klíče pro exportované tabulky
--

--
-- Klíče pro tabulku `competitions`
--
ALTER TABLE `competitions`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `figurants`
--
ALTER TABLE `figurants`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `injuries`
--
ALTER TABLE `injuries`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `result_injuries`
--
ALTER TABLE `result_injuries`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `result_tasks`
--
ALTER TABLE `result_tasks`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `stations`
--
ALTER TABLE `stations`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `tasks`
--
ALTER TABLE `tasks`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `teams`
--
ALTER TABLE `teams`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `threathment_procedures`
--
ALTER TABLE `threathment_procedures`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `tokens`
--
ALTER TABLE `tokens`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `token_UNIQUE` (`token`);

--
-- Klíče pro tabulku `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);

--
-- Klíče pro tabulku `user_templates`
--
ALTER TABLE `user_templates`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email_UNIQUE` (`email`),
  ADD UNIQUE KEY `phoneNumber_UNIQUE` (`phoneNumber`);

--
-- AUTO_INCREMENT pro tabulky
--

--
-- AUTO_INCREMENT pro tabulku `competitions`
--
ALTER TABLE `competitions`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT pro tabulku `figurants`
--
ALTER TABLE `figurants`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pro tabulku `injuries`
--
ALTER TABLE `injuries`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pro tabulku `result_injuries`
--
ALTER TABLE `result_injuries`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pro tabulku `result_tasks`
--
ALTER TABLE `result_tasks`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pro tabulku `stations`
--
ALTER TABLE `stations`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT pro tabulku `tasks`
--
ALTER TABLE `tasks`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pro tabulku `teams`
--
ALTER TABLE `teams`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pro tabulku `threathment_procedures`
--
ALTER TABLE `threathment_procedures`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pro tabulku `users`
--
ALTER TABLE `users`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT pro tabulku `user_templates`
--
ALTER TABLE `user_templates`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
