mergeInto(LibraryManager.library, {
	ShowAdv: function ()
	{
		ysdk.adv.showFullscreenAdv({
			callbacks: {
				onClose: function (wasShown)
				{
					console.log("showFullscreenAdv -> onClose");
				},
				onError: function (error)
				{
					console.log("showFullscreenAdv -> onError");
					console.log(error);
				}
			}
		});
	},
	ShowRewardAdv: function ()
	{
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
		});
	},
	IsMobile: function ()
	{
		var isMobile = Module.SystemInfo.mobile
		console.log("JSLib: isMobile: ", isMobile);
		return isMobile;
	},
	CheckAuth: function ()
	{
		const auth = player.getMode() !== 'lite';
		console.log("JSLib: CheckAuth: ", auth);
		return auth;
	},
	AuthPlayer: function ()
	{
		if (player.getMode() === 'lite')
		{
			ysdk.auth.openAuthDialog().then(() =>
			{
				console.log("JSLib: OnAuth");
				initPlayer().then(p =>
					myGameInstance.SendMessage("Yandex", "OnAuth", 1));
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
	},
	GetLeaderboard: function ()
	{
		const param = player.getMode() === 'lite' ?
			{ quantityTop: 14 } :
			{ quantityTop: 5, includeUser: false, quantityAround: 3 }
		lb.getLeaderboardEntries('score', param)
			.then(res =>
			{
				console.log("JSLib: GetLeaderboard");
				console.log(res);
				const data = { Records: res.entries.map(v => ({ ID: v.player.uniqueID, Score: v.score, Rank: v.rank, Avatar: v.player.getAvatarSrc("small"), Name: v.player.publicName, IsPlayer: false })) };
				console.log(data);
				myGameInstance.SendMessage("Yandex", "SetLeaderboard", JSON.stringify(data));
			});
	},
	SetScore: function (score)
	{
		lb.setLeaderboardScore('score', score).then(() =>
		{
			console.log("JSLib: SetScore success");
			myGameInstance.SendMessage("Yandex", "OnScoreUpdated", 1);
		}).catch(e =>
		{
			console.log("JSLib: SetScore error");
			console.log(e);
			myGameInstance.SendMessage("Yandex", "OnScoreUpdated", 0);
		})
	},
	GetScore: function ()
	{
		lb.getLeaderboardPlayerEntry('score').then(res =>
		{
			console.log("JSLib: GetScore success");
			myGameInstance.SendMessage("Yandex", "SetCurScore", res.score);
		}).catch(e =>
		{
			console.log("JSLib: GetScore default");
			console.log(e);
			myGameInstance.SendMessage("Yandex", "SetCurScore", 0);
		});
	},
	GetPlayerData: function ()
	{
		lb.getLeaderboardPlayerEntry('score').then(res =>
		{
			console.log("JSLib: GetScore success");
			const data = { ID: res.player.uniqueID, Score: res.score, Rank: res.rank, Avatar: res.player.getAvatarSrc("small"), Name: res.player.publicName, IsPlayer: true }
			myGameInstance.SendMessage("Yandex", "SetPlayerData", JSON.stringify(data));
		}).catch(e =>
		{
			console.log("JSLib: GetScore default");
			console.log(e);
			const data = { ID: "", Score: 0, Rank: 0, Avatar: "", Name: "", IsPlayer: false }
			myGameInstance.SendMessage("Yandex", "SetPlayerData", JSON.stringify(data));
		});
	},
});