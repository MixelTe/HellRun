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
					myGameInstance.SendMessage("Yandex", "GetReward");
				},
				onClose: () =>
				{
					myGameInstance.SendMessage("Yandex", "CancelReward");
				},
				onError: (e) =>
				{
					myGameInstance.SendMessage("Yandex", "CancelReward");
				}
			}
		});
	},
	IsMobile: function ()
	{
		return Module.SystemInfo.mobile;
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
			}).catch(() =>
			{
				myGameInstance.SendMessage("Yandex", "OnAuth", 0);
			});
		}
		else
		{
			myGameInstance.SendMessage("Yandex", "OnAuth", 1);
		}
	},
	GetLeaderboard: function ()
	{
		const param = player.getMode() === 'lite' ?
			{ quantityTop: 14 } :
			{ quantityTop: 10, includeUser: true, quantityAround: 3 }
		lb.getLeaderboardEntries('score', param)
			.then(res =>
			{
				console.log("GetLeaderboard");
				console.log(res);
				const data = res.entries.map(v => ({ score: v.score, rank: v.rank, avatar: v.player.getAvatarSrc("small"), name: v.player.publicName }));
				console.log(data);
				myGameInstance.SendMessage("Yandex", "SetLeaderboard", JSON.stringify(data));
			});
	},
	SetScore: function (score)
	{
		ysdk.isAvailableMethod('leaderboards.setLeaderboardScore').then(v =>
		{
			console.log("JSLib: SetScore");
			if (v)
				lb.setLeaderboardScore('score', score);
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
			myGameInstance.SendMessage("Yandex", "SetCurScore", 0);
		});
	},
});