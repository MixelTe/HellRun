const YandexApiLib = {
	OnStart: function ()
	{
		console.log("Game started!");
		CloseSpinner();
	},
	ShowAdv: function ()
	{
		Try(() =>
			ysdk.adv.showFullscreenAdv({
				callbacks: {
					onClose: function (wasShown)
					{
						console.log("showFullscreenAdv -> onClose");
						myGameInstance.SendMessage("Yandex", "OnAdvClosed");
					},
					onError: function (error)
					{
						console.log("showFullscreenAdv -> onError");
						console.log(error);
						myGameInstance.SendMessage("Yandex", "OnAdvClosed");
					}
				}
			}),
			() => myGameInstance.SendMessage("Yandex", "OnAdvClosed"));
	},
	ShowRewardAdv: function ()
	{
		Try(() =>
			ysdk.adv.showRewardedVideo({
				callbacks: {
					onOpen: () =>
					{
						console.log('Video ad open.');
					},
					onRewarded: () =>
					{
						console.log('showRewardedVideo -> onRewarded');
						myGameInstance.SendMessage("Yandex", "OnReward", 1);
					},
					onClose: () =>
					{
						console.log('showRewardedVideo -> onClose');
						myGameInstance.SendMessage("Yandex", "OnReward", 0);
					},
					onError: (e) =>
					{
						console.log('showRewardedVideo -> onClose');
						console.log(e);
						myGameInstance.SendMessage("Yandex", "OnReward", -1);
					}
				}
			}),
			() => myGameInstance.SendMessage("Yandex", "OnReward", -1));
	},
	IsMobile: function ()
	{
		return Try(() =>
		{
			var isMobile = Module.SystemInfo.mobile
			console.log("JSLib: isMobile: ", isMobile);
			return isMobile;
		}, () => false);
	},
	CheckAuth: function ()
	{
		return Try(() =>
		{
			const auth = player.getMode() !== 'lite';
			console.log("JSLib: CheckAuth: ", auth);
			return auth;
		}, () => false);
	},
	AuthPlayer: function ()
	{
		Try(() =>
		{
			if (player.getMode() === 'lite')
			{
				ysdk.auth.openAuthDialog().then(() =>
				{
					console.log("JSLib: OnAuth");
					initPlayer()
						.then(() => myGameInstance.SendMessage("Yandex", "OnAuth", 1))
						.catch(() => myGameInstance.SendMessage("Yandex", "OnAuth", 0));
				}).catch(e =>
				{
					console.log("JSLib: OnAuth: Error");
					console.log(e);
					myGameInstance.SendMessage("Yandex", "OnAuth", 0);
				});
			}
			else
			{
				console.log("JSLib: OnAuth: Already authorized");
				myGameInstance.SendMessage("Yandex", "OnAuth", 1);
			}
		}, () => myGameInstance.SendMessage("Yandex", "OnAuth", 0));
	},
	GetLeaderboard: function ()
	{
		Try(() =>
		{
			const param = player.getMode() === 'lite' ?
				{ quantityTop: 14 } :
				{ quantityTop: 5, includeUser: true, quantityAround: 6 }
			lb.getLeaderboardEntries("scores", param)
				.then(res =>
				{
					console.log("JSLib: GetLeaderboard");
					console.log(res);
					const data = {
						Records: res.entries.map(v => ({
							ID: v.player.uniqueID,
							Score: v.score,
							Rank: v.rank,
							Avatar: v.player.scopePermissions.avatar != "forbid" ? v.player.getAvatarSrc("small") : "",
							Name: v.player.publicName,
							IsPlayer: false,
							RatedGame: v.extraData.includes("RatedGame"),
							WasTop: v.extraData.includes("WasTop"),
							WasFirst: v.extraData.includes("WasFirst"),
							GamesPlayed: parseInt(v.extraData.split(";")[0]) ?? 0,
							HasGear: v.extraData.includes("HasGear"),
						}))
					};
					console.log(data);
					myGameInstance.SendMessage("Yandex", "SetLeaderboard", JSON.stringify(data));
				});
		}, () => myGameInstance.SendMessage("Yandex", "SetLeaderboard", JSON.stringify({ Records: [] })));
	},
	SetScore: function (score, games_played, rated_game, was_top, was_first, has_gear)
	{
		const extraData = [
			`${games_played}`,
			rated_game ? "RatedGame" : "",
			was_top ? "WasTop" : "",
			was_first ? "WasFirst" : "",
			has_gear ? "HasGear" : "",
		].join(";");
		Try(() =>
			lb.setLeaderboardScore("scores", score, extraData).then(() =>
			{
				console.log("JSLib: SetScore success");
				myGameInstance.SendMessage("Yandex", "OnScoreUpdated", 1);
			}).catch(e =>
			{
				console.log("JSLib: SetScore error");
				console.log(e);
				myGameInstance.SendMessage("Yandex", "OnScoreUpdated", 0);
			}),
			() => myGameInstance.SendMessage("Yandex", "OnScoreUpdated", 0));
	},
	GetScore: function ()
	{
		// Not used
		Try(() =>
			lb.getLeaderboardPlayerEntry("scores").then(res =>
			{
				console.log("JSLib: GetScore success");
				myGameInstance.SendMessage("Yandex", "SetCurScore", res.score);
			}).catch(e =>
			{
				console.log("JSLib: GetScore default");
				console.log(e);
				myGameInstance.SendMessage("Yandex", "SetCurScore", 0);
			}),
			() => myGameInstance.SendMessage("Yandex", "SetCurScore", 0));
	},
	GetPlayerData: function ()
	{
		const emptyData = { ID: "", Score: 0, Rank: 0, Avatar: "", Name: "", IsPlayer: true, HasSavedRecord: false, RatedGame: false, WasTop: false, WasFirst: false, HasGear: false }
		Try(() =>
			lb.getLeaderboardPlayerEntry("scores").then(res =>
			{
				console.log("JSLib: GetScore success");
				const data = {
					ID: res.player.uniqueID,
					Score: res.score,
					Rank: res.rank,
					Avatar: res.player.scopePermissions.avatar != "forbid" ? res.player.getAvatarSrc("small") : "",
					Name: res.player.publicName,
					IsPlayer: true,
					HasSavedRecord: true,
					RatedGame: res.extraData.includes("RatedGame"),
					WasTop: res.extraData.includes("WasTop"),
					WasFirst: res.extraData.includes("WasFirst"),
					GamesPlayed: parseInt(res.extraData.split(";")[0]) ?? 0,
					HasGear: res.extraData.includes("HasGear"),
				}
				console.log(data);
				myGameInstance.SendMessage("Yandex", "SetPlayerData", JSON.stringify(data));
			}).catch(e =>
			{
				console.log("JSLib: GetScore default");
				console.log(e);
				myGameInstance.SendMessage("Yandex", "SetPlayerData", JSON.stringify(emptyData));
			}),
			() => myGameInstance.SendMessage("Yandex", "SetPlayerData", JSON.stringify(emptyData)));
	},
	GetLang: function ()
	{
		return Try(() =>
		{
			const langStr = ysdk.environment.i18n.lang;
			const ru = ["ru", "be", "kk", "uk", "uz"];
			const lang = ru.indexOf(langStr) >= 0 ? 0 : 1;
			console.log("JSLib: GetLang: ", langStr, lang);

			return lang;
		}, () => 0)
	},
	UpdateUserStatus: function (rank, record, volume_sound, volume_music, auth, language, rated_game, was_top, was_first, games_played, has_gear)
	{
		const params = {
			rank,
			record,
			volume_sound,
			volume_music,
			auth,
			language: language == 0 ? "ru" : "en",
			rated_game,
			was_top,
			was_first,
			games_played,
			has_gear,
		};
		console.log("JSLib: UpdateUserStatus: ", params);
		Try(() =>
			ym(MID, 'userParams', params));
	},
	SendMetrika: function (goal)
	{
		const goalStr = UTF8ToString(goal);
		console.log("JSLib: SendMetrika: ", goalStr);
		Try(() =>
			ym(MID, 'reachGoal', goalStr));
	},
	CanReview: function ()
	{
		console.log("JSLib: CanReview");
		Try(() => ysdk.feedback.canReview().then(({ value, reason }) =>
		{
			console.log("JSLib: CanReview:", value, reason);
			myGameInstance.SendMessage("Yandex", "OnCanReview", value ? 1 : 0, reason == "GAME_RATED" ? 1 : 0);
		}),
			() => myGameInstance.SendMessage("Yandex", "OnCanReview", 0, 0));
	},
	RequestReview: function ()
	{
		console.log("JSLib: RequestReview");
		Try(() => ysdk.feedback.requestReview().then(({ feedbackSent }) =>
		{
			console.log("JSLib: RequestReview:", feedbackSent);
			myGameInstance.SendMessage("Yandex", "OnReviewRequested", feedbackSent ? 1 : 0);
		}),
			() => myGameInstance.SendMessage("Yandex", "OnReviewRequested", 0));
	},
	$Try: function (f, ef)
	{
		try
		{
			return f();
		}
		catch (e)
		{
			console.error("JSLib error: ", e);
			if (ef)
				return ef();
		}
	}
}

autoAddDeps(YandexApiLib, '$Try');
mergeInto(LibraryManager.library, YandexApiLib);
