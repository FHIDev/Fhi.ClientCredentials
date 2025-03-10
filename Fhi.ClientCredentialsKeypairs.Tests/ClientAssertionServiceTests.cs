﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Fhi.ClientCredentialsKeypairs.Tests
{
    public class ClientAssertionServiceTests
    {
        [Test]
        public async Task ClientAssertion_WithNoAlgotithm()
        {
            var assertionService = new ClientAssertionService();

            var assertion = await assertionService.CreateClientAssertionJwtAsync(
                "https://issuer",
                "client_id",
                $"{{\"d\":\"Q4x8XiZ3JKn0-ijW-H9plfw7QF4VLK43jHxYtPJvX6GcBuEk_rMedziQuqbBCZrK6aWVspnYS6dQtj33Z2TtSkXu2gy_1xR2nR8h9XeZ6h6QRbL9bj1Qxrk70ry7bXz5WIjyyuPmY73aPw9OFrZ_NDeUQjiEofzTHkr86ZIVjAmNLarVufG9P2V6fz14wwHc3aLBVgUt7Rxx5sFOQR30zYGpd1BH-xK6ykA6n6BdaIc4luWw_SkmVowwO4toScj07qoAYTUR4IFQHYt7sQZNufFG89nB-v_Er0a2tRvtME2NnU_4rn4ea1yyGFlYH_6Amtb8u4-TAeOESjrMw9ylBkvb6vIvtqT0lQdBJJEPI_Hx-655ElvO4zT48HBS6oVZHCARN17d7pQWrnxiSusYEdM9RwJET57ieVayo-baQe3NOvj2Y5V2H034cWCJt_DTh7ye9RXD4gtMnHDQ-tgV6ztwW8GkGvbJzXUnkqGXUvKqjeJAnOc2Ahoxpc-9cnMnW2DrwPnI0f9Jsq0n3hQyqwnnyimIeZn32WVe2Q4XC7d_VB21E8oDZhdeUlxuTZX-foTrYB3xvDKB6tLCaaMbfpzvUsSfSYqbAXQfqhQWosyt7w-ZIYJOY05fWspR3mlpo5IMGkaDp8clvz51f8zdMfSYFTml4e_zjoduvlz2wyE\",\"dp\":\"GsxR4BGYEb460zNcX1_SROtq2zVG8IfYVFy-3pFQvmerfiRJr0uuWvZ1WCtqalXKV33ACdf5njmkKdA-z-RbH07axt52b8SQZOTQ8527i5p_zJ6QGp6Lw4iuepAX64T_POtqmUDKcusIOGxxZbC_SjVr7dtYHVWuqPZlNjFbqQpWcerQybQvsyBeVDzDYkcdM7dhW8wXuIPDukU0zkgKBvW-23LR6dN9t7zh3suLdhkaV381-lODAU_U02-wIhXwDcHKi_8a1dMtMKQFxKyngp0d3P8R5hDT71UOttD0zMxt6HB_c6cwLmOYPclYXsK0-bIIDgJq7rERIA6KF0GqaQ\",\"dq\":\"CSI90DDVZFFj0DWpUXcpWjH5NOP7Yca9dTeFkWqnhmpS_XOMNvCLa9pyTO7o-Iw-aRbVhlpyIQN4pSdMmjnW5eEpBg8zsd8LcV8gkv9sL08bL-8dWcqy5kD9pgBEK49HgiobTWpdKd02PErgXbY28hWRx4JafVRjk7PkRXD4yJjK_qJ-fwlY51K0ynAIX4L7C14LW7AVYp4QkRjaHd_O1CRorVijR_N_sEvfa1jfZHNtBmgaUbJxn_4rYVZfehu5nbqoXLB4VqJBwVf26rNyT-fxMbAW4OH0ubjWrcTCedfAIMMegbXG7cHxrrbAL-50PggVWWjxbKAt0gBoN5KNIQ\",\"e\":\"AQAB\",\"kid\":\"-JYdQcqGy0Qmbpv6pX_2EdJkGciRu7BaDJk3Hz4WdZ4\",\"kty\":\"RSA\",\"n\":\"iu9EmQLoIJBPBqm3jLYW4oI8yLkOxvKg-OagE8HlzP-RQnDXH9hBe2cTRZ3oNqG1viWmv6-dxNtKU1QxOpezWLx-N-AJ7dIlXTMUGkCheHUorPSzakeBUOCHtvT1Tdv9Mzue9fVt3JxpPX6mQNlsOzwk9L8HmbgojMcApKmQcfNriVV72byLuaAoh9fcXSNm6TUuwO5cPmnHgS5B5Hfe5P0OIte027oZyjPiYm-QbV4YJNjwwwZnPvkLaRjw6L8sV5TAOLvNQIt63OpF8UHPjBsM8LJHdHFUMgx2BaMaJC8tNCi_8UWGG59sd4-_vJC78s3wZNEGL6OwCngpF7NLwaP9Zqxx8DDkOY71MvvcAyu4i0D6_8A8_qewLvb_SPxNpCe8zH5MJIKNJB38InWd8FpvpbPuEJt4oK1gfUBWLWQ39YIHzodKhkN-qAXYWGyzJ2nJdNIMAclefw251Cvjcyf3gmVATXDBAo-piUJIGXC3y7yqfyMupe_4oRe69DFBZTecXSLEdbAbUtiaH9r4rY5oeYCiZ70wcFcieHFZLwfleCPm5Cz8rEQxK8KjMis2kb1aRxVytTj_0pOkw1HEJU1tv_TWmD136RgoRtiqnVoxmCM6Q4XxXrOnGMPZR0_ScYHdW_YjDgnJBQykAbzW0nC47d3KSotktz1cPejo5_s\",\"p\":\"wSqXfloa5ikf6d_G3N2x-IBjTEB7mibEif9qTNED7x4f7J8_vB_rdrsxoTCdY3R5j2BIS3XKyiTIkonfV5mYQvSu9RwxgX4ImcTsTXMxewuyQ031OJz0ruvlgvHELzdgkyo4q4NQaDigejfp_DsoEM0nLHeNXYfTz_AuTlkG8BA0JzrBK0aoBPUdwB1_WoJoJYVhST-B_PHQ3eaFOQNGXQXoYc6YZt4WKmu3WModjezdqnKVKSaORTuG5-mTQLS2jxZr1XlEDXWN53tH_Oon2WMDGbSCVz_qYVZUWorbjvUmxJnYP0H-lIxLRfxYE68DnYlXSWOzcfpdD6VIlP972w\",\"q\":\"uCCszPIHfiHe5UOa0poW8WELpL_YItCdK_AnwulRHOM2FIQbmWBVZBMRguQJuMCvjIAWdQNEOrZYt1BhIknziUHSvSnLU0qszJ_ZHByZS54834CIxc-0etVkKbpnJGpjzgucvmEPNzrUko2ip512iOY9WTqB54Awg56rS-3oBw2_iqEzioAI30pD9-AX5xDRBEJcRJ8mPxw0iSDGVkxwIQNLBlML79cGNGjdrzsJyjPuMMDTLadHnRSAybmqhVTJYwEH5t37_f_fho2aoPu_sW65LdoW6Z-V042xnieMm3XhA8yZxonao2LH6Bh7Xk7qinWYpYuF6UpKqhtrpI8OYQ\",\"qi\":\"Z7GWLxO3q2BolVFaOjuskhYc0V4GZ-b-SA2Rv110HovMDatlUGZKgoAOponFcvEddJSNf7stRM35HWiTN9W5iSUP4VLqAlJ2W7ftIsRJv0D-Lcs4HoXTAHcny36j0eEzhNLUYwfS9Y7ICWEWHv_WTG8Iz_I87JKLCnjGutZDmsM_fHDPmUkv7Pf2GG9r9hzS8uylC43ik4gfrp0Hm_6rAHKB4EHVHfYu51zl9yLkPgqq8ycHi0tF7VmVtDUsIMJdz7nlGOCS-468WI95dAfdfTC8v9JKXj9JL3ylM3dDbiC3m0p-rpaM2VzuO4OrMk-jWFCuYbDCYS-bcYFG4XwmtA\"}}");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(assertion);

            Assert.Multiple(() =>
            {
                Assert.That(token.Issuer, Is.EqualTo("client_id"), "Issuer mismatch");
                Assert.That(token.Claims.Count, Is.EqualTo(7), "Unexpected claim count");

                var audClaim = token.Claims.SingleOrDefault(x => x.Type == "aud");
                Assert.That(audClaim, Is.Not.Null, "Missing 'aud' claim");
                Assert.That(audClaim!.Value, Is.EqualTo("https://issuer"), "Invalid 'aud' claim value");

                var subClaim = token.Claims.SingleOrDefault(x => x.Type == "sub");
                Assert.That(subClaim, Is.Not.Null, "Missing 'sub' claim");
                Assert.That(subClaim!.Value, Is.EqualTo("client_id"), "Invalid 'sub' claim value");

                Assert.That(token.Claims.Any(x => x.Type == "jti"), Is.True, "Missing 'jit' claim");
                Assert.That(token.Claims.Any(x => x.Type == "nbf"), Is.True, "Missing 'nbf' claim");
                Assert.That(token.Claims.Any(x => x.Type == "iat"), Is.True, "Missing 'iat' claim");
                Assert.That(token.Claims.Any(x => x.Type == "exp"), Is.True, "Missing 'exp' claim");

                var typ = token.Header.SingleOrDefault(x => x.Key == "typ");
                Assert.That(typ.Value, Is.EqualTo("client-authentication+jwt"));

                var alg = token.Header.SingleOrDefault(x => x.Key == "alg");
                Assert.That(alg.Value, Is.EqualTo(SecurityAlgorithms.RsaSha256));

                var kid = token.Header.SingleOrDefault(x => x.Key == "kid");
                Assert.That(kid.Value, Is.EqualTo("-JYdQcqGy0Qmbpv6pX_2EdJkGciRu7BaDJk3Hz4WdZ4"));
            });
        }

        [Test]
        public async Task ClientAssertion_WithAlgotithmSet()
        {
            var assertionService = new ClientAssertionService();
            var assertion = await assertionService.CreateClientAssertionJwtAsync(
              "https://issuer",
              "client_id",
              $"{{\"alg\":\"PS512\",\"d\":\"BHnwAEpzq8XkCTMwR0hjhsrTz7DXLh5XPvh4q4rg0s-eA8unSn1-bvSuqeWb8MxSy9btJIrLiJ0A7Q-QC7uN-UP7L3s_5BunTz5CXx-rnGm0Q_E2MUtsUxVcsn-dcQSbqOWzA_vblGpdZJpbE6PspdukXR-OcdDzqy-J6YcreKVVrT3o-b5raI9bjcvUppJNK2CtEr-dYjyVWotML8e_GIf2wU2Gg_KBnX46JUp9DoedzMCthQX63yflYw-BMTFLJ-MvecrJQlQsrMjkNj8345C_vJU-gKIWFGAzhik7zwjxD68XUu7tV6-2Od0KjnFEQ7ARBZ9MiN3nnOIm42zLgvO0i0KSzqeW-QXbWKr1hEkWBenuXwwH3zzeU2WoLuXjxDIEQ7Mjava6U7AXBspbqgGg9wqyzeRCZapKXLx-nCnzTV5pM1RE5rWHJAiU6U2rPWEQGAiDT0art7E_4aa87QmL_fDXCAguPk7YniJUZ6_pln5ZpeSEza-ot-8KX8kVPJRmDDrSDoGE8aHJ39PtFEbuHQ_YQwtpg5Mj4px7l5nvKalox3z-JlgMGcx7dvEnENdukiIOYn0sPsbGv7g0irvDcIG7nBMUsm8memv4IT-ITTsyXt3IqCxgYF2VkFgy86TwFg0LA8NDGXhhdLa0aPWLNFGIvboBUEyF7aVshsE\",\"dp\":\"DoNAx4hRPTKd5o9LoIaqfuyKY3pjVIWr4GuYL1QTZgKvXEDNQbdd35hQ2eJ8rQk_SiXh7v10t_3B0ZBx4L8MEP9weQjI2g4G_rjregv7Gju0wC1NecWExgh3MKYvmpY1ZYIPFM7e9CuZvQhr2m0nnRUapQflWYGXwj56DZbejb7EmdEknTqtlW6qrTzOSR66JPmaysZUpmjPTXH2N4c9fyzDF91iX0nwJytxltuhw6yqQb5EGTu9S7fDHm6TCor-R8hpeD0ix8jB40JYCJL_qmVqkqZnBgVXH8uX-OfZNbl8TO_5JsRAD5tFtRYgzLvnYwPxcrc_Qe6UN5fk3ZaHYQ\",\"dq\":\"cUiwCI71k_BLZN7q7F1RpF77jFoYPrti1WgQUVr2kwAjOJ-FL78MtGMIDudDdtkcRFDnIAYm423isZ_bFm3ASqOgFfxl1eJClajsqQTnmS6nkvrCVKS0EqQNEUpdCpFPnWkCg9SFgMYiVRRYrX4KONKx-C4pmedxZZthf6HLC0Pee7YYlhV6nIuPLM97ASjxmLxXAOt3aBXA63-RSSBYc1cjur6wtbcadbQEbGyFbz02XO6aimeYb3am6-Es841FZVbRLcRdwL2vMZTRARhLO9XKWjbHrxf45pTdNSVdfqCAvMRt25thZ5fkyI9yw7Hck2ko0HivnOhkPA_58-v-fw\",\"e\":\"AQAB\",\"kid\":\"Yho5xPwtUe9T20E2h9zbVeFvNeVn6307OyEHGscsfnc\",\"kty\":\"RSA\",\"n\":\"vp7E_JUJL6rNHVBBnfgmdH-EqRyW3XzRJLbqi1G8FLMfAht19dE-ySBfk5cteqNHuG1ImlrV0S5VLX4a1ISxH8OG0FUQBz5IAkHOqQKeCd3Yrzy8tz6lwrvuAwvM3z0pAQ7KOAk-F0dUKeBXfsCr9hFWIEHMqZF2n-OSZFFXTHwlBhX-R20lQdUaTr5ejI0DxcEyo3ZKfxRkU0d7cEUC371YNJxUvyFcpvsdsezDHzfHgVO3F7QhdRDkDB_6ywSMlsMt6BchSZhepEYHgiadKDHJVCpubjdYElgk_PctyeW752ghmuftI5suq-Ah2Qvth0p5C4ftZbU52gjxymKhUPJ-ozM4uYKb9kYtgDAbUSojDYUob-eC9mRaCw9R_IkP3C1FvDifJz_caqSBNN4AHYXm97CUpEca6wUh9ReyYSIYAhyqXbhq0CButB3e1an2X-bW_eET-bC42YWPajz9FVnrKBHclDRAQTENASx0LuLM_j9aHTNPFeX6eRaOeM1GI_y_9WtWUWeCEHJfRV9UIwS4Vta9FNa7UharkY19sH9xNenCwoFVfAj8vGSdcLklqnQK1dmHXTgEbLRrilrdLH0syH7nIK20-ZQ5BWLBa-pcAARCBkSro9gmGbzpQMd1teUAB4e4Y8Dlm_JDkUwqf1yNqSPo_FVqHJnJEybnNbs\",\"p\":\"8TeYTPFP5DPhsP-wACYczNdgi4J8tydve4Sucu87SERj6nSS5Pd32XJcueca6bnFqG1W3JN9uWRtSjoBw_B4gQoHbqYzeal1PRbNOfFmylv_eRbEJM_tA0MEbB_1qYHqCXGXqBaUGNZghRtkGtCK01AqNcsLtm-fo_ONvGhoswUFjn6Ak3bNCngeYPbxxs0-5KZz-yN6Ct9S7Q8bCmru8524fj7c608biQfnvNEZeHbPAgvvmznAukYsa-K_3I2XIZx8cFwhTSqBM8KqKeD7OJ69dLvfTiDtLjVaeSOFhfxdpkqKNXWiG9_h5eOvazy2f9vug1LoYPzUDozq3Z2BoQ\",\"q\":\"yk1ejMagR3jHbbHNJHMVqEOw-ZnPhl6kj78xspQ7gI89aX-I6AMn-T7nFuV5XtMB--ugjSsjDL_8M6pmSwzRv0BT4vwOuHqp1DsynzzMGDw4H6hsM8764dvn5sWjDHtfd49wq3uqvQ1PcL0NhlJMA0yFiMlp9DDE3FMr7s6Yl9a3XAUi29FS3TaqleXP4uWsEQi9Guwg7AlpA3FBY0SlA6Gnk10n_ow54DG8xYeSM4pt6NyJ_Fh_yaOlFTaztlMWOMT7hMrnpEeudpmxJX1Dq_4Bmrwz6XrrDlHEbYkx5ZqjnbwlI4H_pOECflS4w_b9RHbd72wdz6id54RST-mx2w\",\"qi\":\"UezCuzjaxIuAuUvoNs3xl5glfCBs6VcURgKcyGhWkIcDtjPQFrJFGmPI8P-RpqWRhk3DLJon6NLsA_rgUE-9cQZWx3vS86TXC2IzrALSSyOoqIGzWnFAJRSiQFExB3HNJm13MGB5RO1b3cRWDj59UChBQFwvA5LJ_rwOV3Hd6JWilROC_4h3bB9tCw0-8djK9dBy83MeKv434pGct00fSO0S6URcB_xqaJG78_YES0FNk3kDxOYwXFKKwkBFJRvyvpfXyzZKnxnzbIyFZwoORe-9LQOhkLGTgkmZVbO7yfiEANW7s9xcL4iGYWXCkRN2ELjtAC4CAcpV7jsAjZZvvw\"}}");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(assertion);

            Assert.Multiple(() =>
            {
                Assert.That(token.Issuer, Is.EqualTo("client_id"), "Issuer mismatch");
                Assert.That(token.Claims.Count, Is.EqualTo(7), "Unexpected claim count");

                var audClaim = token.Claims.SingleOrDefault(x => x.Type == "aud");
                Assert.That(audClaim, Is.Not.Null, "Missing 'aud' claim");
                Assert.That(audClaim!.Value, Is.EqualTo("https://issuer"), "Invalid 'aud' claim value");

                var subClaim = token.Claims.SingleOrDefault(x => x.Type == "sub");
                Assert.That(subClaim, Is.Not.Null, "Missing 'sub' claim");
                Assert.That(subClaim!.Value, Is.EqualTo("client_id"), "Invalid 'sub' claim value");

                Assert.That(token.Claims.Any(x => x.Type == "jti"), Is.True, "Missing 'jit' claim");
                Assert.That(token.Claims.Any(x => x.Type == "nbf"), Is.True, "Missing 'nbf' claim");
                Assert.That(token.Claims.Any(x => x.Type == "iat"), Is.True, "Missing 'iat' claim");
                Assert.That(token.Claims.Any(x => x.Type == "exp"), Is.True, "Missing 'exp' claim");

                var typ = token.Header.SingleOrDefault(x => x.Key == "typ");
                Assert.That(typ.Value, Is.EqualTo("client-authentication+jwt"));

                var alg = token.Header.SingleOrDefault(x => x.Key == "alg");
                Assert.That(alg.Value, Is.EqualTo(SecurityAlgorithms.RsaSsaPssSha512));

                var kid = token.Header.SingleOrDefault(x => x.Key == "kid");
                Assert.That(kid.Value, Is.EqualTo("Yho5xPwtUe9T20E2h9zbVeFvNeVn6307OyEHGscsfnc"));
            });


        }
    }
}
