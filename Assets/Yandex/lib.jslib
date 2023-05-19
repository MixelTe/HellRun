const YandexApiLib = {
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
				{ quantityTop: 5, includeUser: false, quantityAround: 3 }
			lb.getLeaderboardEntries("scores", param)
				.then(res =>
				{
					console.log("JSLib: GetLeaderboard");
					console.log(res);
					const data = { Records: res.entries.map(v => ({ ID: v.player.uniqueID, Score: v.score, Rank: v.rank, Avatar: v.player.getAvatarSrc("small"), Name: v.player.publicName, IsPlayer: false })) };
					console.log(data);
					myGameInstance.SendMessage("Yandex", "SetLeaderboard", JSON.stringify(data));
				});
		}, () => myGameInstance.SendMessage("Yandex", "SetLeaderboard", JSON.stringify({ Records: [] })));
	},
	SetScore: function (score)
	{
		Try(() =>
			lb.setLeaderboardScore("scores", score).then(() =>
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
		const emptyData = { ID: "", Score: 0, Rank: 0, Avatar: "", Name: "", IsPlayer: true, HasSavedRecord: false }
		Try(() =>
			lb.getLeaderboardPlayerEntry("scores").then(res =>
			{
				console.log("JSLib: GetScore success");
				const data = { ID: res.player.uniqueID, Score: res.score, Rank: res.rank, Avatar: res.player.getAvatarSrc("small"), Name: res.player.publicName, IsPlayer: true, HasSavedRecord: true }
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
	UpdateUserStatus: function (rank, record, volume_sound, volume_music, auth, language)
	{
		const params = {
			rank,
			record,
			volume_sound,
			volume_music,
			auth,
			language: language == 0 ? "ru" : "en",
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
